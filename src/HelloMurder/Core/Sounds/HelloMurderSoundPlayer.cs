using Murder.Core.Sounds;
using Murder.Diagnostics;
using HelloMurder.Core.Sounds.Fmod;
using System.Diagnostics;
using Murder;
using Murder.Serialization;
using Newtonsoft.Json.Linq;

namespace HelloMurder.Core.Sounds
{
    /// <summary>
    /// This is the sound player, which relies on fmod.
    /// The latest version tested was "2.02.11"
    /// </summary>
    public partial class HelloMurderSoundPlayer : ISoundPlayer, IDisposable
    {
        public static HelloMurderSoundPlayer Instance => ((HelloMurderSoundPlayer)Game.Sound);
        private readonly static string _bankRelativeToResourcesPath = Path.Join("sounds");

        private Studio? _studio;

        private Dictionary<SoundEventId, Bank> _banks = new();
        private readonly Dictionary<SoundEventId, Bus> _buses = new();
        private readonly Dictionary<SoundEventId, EventDescription> _events = new();
        private readonly Dictionary<SoundEventId, EventInstance> _instances = new();

        private bool _initialized = false;
        private string _resourcesPath = string.Empty;
        
        public void Initialize(string resourcesPath)
        {
            if (_initialized)
            {
                // This was likely called from a refresh call.
                // We simply need to make sure we are refreshing the cache.
                _ = ReloadAsync();

                return;
            }

            _resourcesPath = FileHelper.GetPath(resourcesPath);

            LoadFmodAssemblies();
            InitializeFmod();
            _initialized = true;
        }

        public async Task LoadContentAsync()
        {
            await FetchBanksAsync();
        }

        public async Task ReloadAsync()
        {
            UnloadContent();
            await LoadContentAsync();
        }

        /// <summary>
        /// This will load and initialize the fmod library so we are ready once the game starts.
        /// </summary>
        private void InitializeFmod()
        {
            // *apparently*, there is a requirement to call the core API before calling the studio API?
            // this seems to break Linux scenarios, but i haven't seen this yet.
            FMOD.Memory.GetStats(out int currentAllocated, out int maxAllocated);

            FMOD.RESULT result = FMOD.Studio.System.Create(out FMOD.Studio.System studio);
            if (!FmodHelpers.Check(result, "Unable to create the fmod factory system!")) return;

            result = studio.GetCoreSystem(out FMOD.System core);
            if (!FmodHelpers.Check(result, "Unable to acquire the core system?")) return;

            _studio = new(studio);

            FMOD.Studio.INITFLAGS studioInitFlags = FMOD.Studio.INITFLAGS.NORMAL;

#if DEBUG
            studioInitFlags |= FMOD.Studio.INITFLAGS.LIVEUPDATE;
#endif

            result = studio.Initialize(
                maxchannels: 256,
                studioflags: studioInitFlags,
                flags: FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER,
                extradriverdata: IntPtr.Zero);

            FmodHelpers.Check(result, "Unable to initialize fmod?");



            // okay, *this has to be called last*. I am not sure why, but things started exploding
            // (non-stream files would not play) if this was not the case <_<
            core.SetDSPBufferSize(bufferlength: 4, numbuffers: 32);

            core.GetDriver(out int driver);
            core.GetDriverInfo(driver, out Guid guid, out int rate, out FMOD.SPEAKERMODE mode, out int channels);

            GameLogger.Log($"Fmod running on Driver: {driver}, with Guid: {guid}, system rate: {rate}, SpeakerMode: {mode}, with {channels} channels");
        }

        internal EventInstance? FetchOrCreateInstance(SoundEventId id)
        {
            if (!_events.TryGetValue(id, out EventDescription? description))
            {
                Debug.Assert(_studio is not null);

                description = _studio.GetEvent(id);
                if (description is not null)
                {
                    _events.Add(id, description);
                }
            }

            return description?.CreateInstance();
        }

        public void Update()
        {
            _studio?.Update();
        }

        public ValueTask PlayEvent(SoundEventId id, SoundProperties properties)
        {
            if (properties.HasFlag(SoundProperties.Persist))
            {
                return PlayPersistedEvent(id, properties);
            }

            // Otherwise, this will be played and immediately released.
            using EventInstance? scopedInstance = FetchOrCreateInstance(id);

            bool success = scopedInstance?.Start() ?? false;
            GameLogger.Verify(success, $"Failed to play event {id.EditorName}. Did the event id got updated?");

            return default;
        }

        public ValueTask PlayPersistedEvent(SoundEventId id, SoundProperties properties)
        {
            if (properties.HasFlag(SoundProperties.SkipIfAlreadyPlaying) && _instances.ContainsKey(id))
            {
                // The sound is currently playing already and it is the same as the last one.
                // So we'll just skip playing it again.
                return default;
            }

            if (properties.HasFlag(SoundProperties.StopOtherMusic))
            {
                Stop(id: null, fadeOut: true);
            }

            if (_instances.ContainsKey(id))
            {
                return default;
            }

            EventInstance? instance = FetchOrCreateInstance(id);
            if (instance is not null)
            {
                // This won't be released right away, so we will track its instance.
                _instances.Add(id, instance);
            }

            bool success = instance?.Start() ?? false;
            GameLogger.Verify(success, $"Failed to play event {id.EditorName}. Did the event id got updated?");

            return default;
        }

        public void SetParameter(SoundEventId id, string name, float value)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                bool success = instance.SetParameterValue(name, value);
                GameLogger.Verify(success, $"Failed to find parameter {id.EditorName}.");
            }
            else
            {
                GameLogger.Error($"Missing sound {id.Path}");
            }
        }

        public void SetParameter(SoundEventId id, ParameterId parameterId, float value)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                bool success = instance.SetParameterValue(parameterId.ToFmodId(), value);
                GameLogger.Verify(success, $"Failed to find parameter {id.EditorName}.");
            }
            else
            {
                GameLogger.Error($"Missing sound {id.Path}");
            }
        }

        public void SetGlobalParameter(ParameterId parameterId, float value)
        {
            bool result = _studio?.SetParameterValue(parameterId, value) ?? false;
            if (result)
            {
                return;
            }

            // Otherwise, this may be tied to a sound? Even though it's global...?
            if (parameterId.Owner is SoundEventId soundId &&
                _instances.TryGetValue(soundId, out var instance))
            {
                instance.SetParameterValue(parameterId.ToFmodId(), value);
            }
        }

        public float GetGlobalParameter(ParameterId parameterId)
        {
            return _studio?.GetParameterCurrentValue(parameterId) ?? 0;
        }

        public bool Stop(SoundEventId? id, bool fadeOut)
        {
            if (id is null)
            {
                return StopAll(fadeOut);
            }

            bool succeeded = false;
            if (_instances.TryGetValue(id.Value, out EventInstance? instance))
            {
                succeeded = instance.Stop(fadeOut);
                instance.Dispose();
            }

            _instances.Remove(id.Value);
            return succeeded;
        }

        private bool StopAll(bool fadeOut)
        {
            EventInstance[] sounds = _instances.Values.ToArray();
            _instances.Clear();

            bool succeeded = false;
            foreach (EventInstance instance in sounds)
            {
                succeeded &= instance.Stop(fadeOut);
                instance.Dispose();
            }

            return succeeded;
        }

        public void SetVolume(SoundEventId? id, float volume)
        {
            if (id is null)
            {
                GameLogger.Fail("Unable to find a null bus id.");
                return;
            }

            if (!_buses.TryGetValue(id.Value, out Bus? bus))
            {
                bus = _studio?.GetBus(id.Value);
                if (bus is null)
                {
                    GameLogger.Fail("Invalid bus name!");
                    return;
                }

                _buses.Add(id.Value, bus);
            }

            bus.Volume = volume;
        }

        private void UnloadContent()
        {
            lock (_banksLock)
            {
                ClearCache();

                foreach (Bank bank in _banks.Values)
                {
                    bank.Dispose();
                }

                _banks.Clear();
            }

            foreach (EventDescription @event in _events.Values)
            {
                @event.Dispose();
            }

            foreach (Bus bus in _buses.Values)
            {
                bus.Dispose();
            }

            foreach (EventInstance instance in _instances.Values)
            {
                instance.Dispose();
            }

            _events.Clear();
            _buses.Clear();
            _instances.Clear();
        }

        private void ClearCache()
        {
            _cacheEventDescriptions = null;
            _cacheEventDescriptionDictionary = null;

            _cacheBuses = null;

            _cacheParameters = null;
            _cacheLocalParameters = null;
            _cacheParameterInfo.Clear();
        }

        public void Dispose()
        {
            UnloadContent();

            _studio?.Dispose();
        }
    }
}
