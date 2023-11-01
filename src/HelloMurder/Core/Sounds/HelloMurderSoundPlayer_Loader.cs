using Murder.Diagnostics;
using HelloMurder.Core.Sounds.Fmod;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Murder.Core.Sounds;
using System.Diagnostics.CodeAnalysis;

namespace HelloMurder.Core.Sounds
{
    /// <summary>
    /// This loads library into the process. I guess this is XPlat??
    /// </summary>
    public partial class HelloMurderSoundPlayer
    {
        private readonly object _banksLock = new();

        private ImmutableArray<EventDescription>? _cacheEventDescriptions = default;
        private ImmutableDictionary<SoundEventId, EventDescription>? _cacheEventDescriptionDictionary = default;

        private ImmutableArray<Bus>? _cacheBuses = default;

        private ImmutableArray<ParameterId>? _cacheParameters = default;
        private ImmutableArray<ParameterId>? _cacheLocalParameters = default;
        private readonly Dictionary<ParameterId, ImmutableArray<ParameterLabel>> _cacheParameterInfo = new();

        /// <summary>
        /// Apparently, fmod will return ERR_INTERNAL if we load any banks before the main one.
        /// So we filter this one out afterwards. Anything that has "master" in the name will be considered a priority.
        /// </summary>
        private const string MAIN_BANK = "Master";

        private void LoadFmodAssemblies()
        {
            string fmodPath = Path.Join(_resourcesPath, "fmod", "pc");
            if (!Directory.Exists(fmodPath))
            {
                GameLogger.Error("Unable to find the library for fmod. Sounds will not be loaded.");
                return;
            }

            // This resolves the assembly when using the logger.
            NativeLibrary.SetDllImportResolver(typeof(HelloMurderGame).Assembly,
                (name, assembly, dllImportSearchPath) =>
                {
                    name = Path.GetFileNameWithoutExtension(name);
                    if (dllImportSearchPath is null)
                    {
                        dllImportSearchPath = DllImportSearchPath.ApplicationDirectory;
                    }

                    return NativeLibrary.Load(Path.Join(fmodPath, GetLibraryName(name)));
                });
        }

        private string GetLibraryName(string name, bool loadLogOnDebug = true)
        {
            bool isLoggingEnabled = loadLogOnDebug;

#if !DEBUG
            isLoggingEnabled = false;
#endif

            if (OperatingSystem.IsWindows())
            {
                return isLoggingEnabled ? $"{name}L.dll" : $"{name}.dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                return isLoggingEnabled ? $"lib{name}L.so" : $"lib{name}.so";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return isLoggingEnabled ? $"lib{name}L.dylib" : $"lib{name}.dylib";
            }

            // TODO: Support consoles?
            throw new PlatformNotSupportedException();
        }

        public async Task FetchBanksAsync()
        {
            lock (_banksLock)
            {
                Debug.Assert(_studio is not null);

                if (_banks.Count > 0)
                {
                    // Banks were already loaded. In that case, start by unloading and starting fresh.
                    UnloadContent();
                }
            }

            string path = Path.Join(_resourcesPath, _bankRelativeToResourcesPath);
            if (!Directory.Exists(path))
            {
                // Skip loading sounds.
                GameLogger.Warning($"No sounds found at {_resourcesPath}");
                return;
            }

            // Order such that we start by loading the main banks first.
            IEnumerable<string> orderedBankPaths = Directory.EnumerateFiles(path, "*.bank", SearchOption.AllDirectories)
                .OrderByDescending(p => p.Contains(MAIN_BANK, StringComparison.InvariantCultureIgnoreCase));

            Dictionary<SoundEventId, Bank> builder = new();
            foreach (string bankPath in orderedBankPaths)
            {
                Bank bank = await _studio.LoadBankAsync(bankPath);

                // bank.LoadSampleData();
                builder.Add(bank.Id, bank);
            }

            lock (_banksLock)
            {
                _banks = builder;
            }

            // Clean the cache once the new banks have been loaded.
            ClearCache();
        }

        [MemberNotNullWhen(true, nameof(_cacheEventDescriptions))]
        [MemberNotNullWhen(true, nameof(_cacheEventDescriptionDictionary))]
        private bool InitializeEvents()
        {
            lock (_banksLock)
            {
                if (_banks.Count == 0)
                {
                    return false;
                }

                if (_cacheEventDescriptions is null)
                {
                    var builder = ImmutableArray.CreateBuilder<EventDescription>();

                    foreach (Bank bank in _banks.Values)
                    {
                        builder.AddRange(bank.FetchEvents());
                    }

                    _cacheEventDescriptions = builder.ToImmutable();
                }

                if (_cacheEventDescriptionDictionary is null)
                {
                    var dictionaryBuilder = ImmutableDictionary.CreateBuilder<SoundEventId, EventDescription>();

                    foreach (EventDescription e in _cacheEventDescriptions)
                    {
                        dictionaryBuilder[e.Id] = e;
                    }

                    _cacheEventDescriptionDictionary = dictionaryBuilder.ToImmutable();
                }

                return true;
            }
        }

        /// <summary>
        /// This fetches all the events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<EventDescription> ListAllEvents()
        {
            if (_cacheEventDescriptions is null && !InitializeEvents())
            {
                // Likely it has not been loaded yet.
                return ImmutableArray<EventDescription>.Empty;
            }

            return _cacheEventDescriptions.Value;
        }

        public bool HasEvent(SoundEventId sound)
        {
            if (_cacheEventDescriptionDictionary is null && !InitializeEvents())
            {
                // Likely it has not been loaded yet.
                return false;
            }

            return _cacheEventDescriptionDictionary.ContainsKey(sound);
        }

        public bool HasBus(SoundEventId bus)
        {
            ImmutableArray<Bus>? buses = _cacheBuses;
            if (buses is null)
            {
                buses = ListAllBuses();
            }

            foreach (Bus b in buses)
            {
                if (bus.Equals(b.Id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This fetches all the events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<Bus> ListAllBuses()
        {
            if (_cacheBuses is null)
            {
                var builder = ImmutableArray.CreateBuilder<Bus>();
                foreach (Bank bank in _banks.Values)
                {
                    builder.AddRange(bank.FetchBuses());
                }

                _cacheBuses = builder.ToImmutable();
            }

            return _cacheBuses.Value;
        }

        public ImmutableArray<ParameterId> ListAllParameters()
        {
            ImmutableArray<ParameterId> global = ListAllGlobalParameters();
            ImmutableArray<ParameterId> locals = ListAllLocalParameters();

            return global.AddRange(locals);
        }

        /// <summary>
        /// This fetches all the global events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<ParameterId> ListAllGlobalParameters()
        {
            if (_cacheParameters is null)
            {
                var builder = ImmutableArray.CreateBuilder<ParameterId>();

                HashSet<ParameterId> listedParameters = new();

                ImmutableArray<EventDescription> events = ListAllEvents();
                foreach (EventDescription e in events)
                {
                    ImmutableArray<ParameterId> eventParameters = e.FetchParameters(withFlags: ParameterFlags.Global);
                    builder.AddRange(eventParameters);

                    foreach (ParameterId parameter in eventParameters)
                    {
                        listedParameters.Add(parameter);
                    }
                }

                if (_studio?.FetchParameters() is ImmutableArray<ParameterId> studioParameters)
                {
                    foreach (ParameterId parameter in studioParameters)
                    {
                        if (listedParameters.Contains(parameter))
                        {
                            // Skip global parameters from events.
                            continue;
                        }

                        builder.Add(parameter);
                    }
                }

                _cacheParameters = builder.ToImmutable();
            }

            return _cacheParameters ?? ImmutableArray<ParameterId>.Empty;
        }

        public FMOD.Studio.PARAMETER_DESCRIPTION? FetchParameterDescription(ParameterId id)
        {
            if (_cacheEventDescriptionDictionary is null && !InitializeEvents())
            {
                // Likely it has not been loaded yet.
                return null;
            }

            if (id.Owner is not SoundEventId sound)
            {
                return _studio?.FetchParameterDescription(id);
            }

            if (!_cacheEventDescriptionDictionary.TryGetValue(sound, out EventDescription? @event))
            {
                return null;
            }

            return @event.FetchParameterDescription(id);
        }

        public ImmutableArray<ParameterLabel> ListAllParameterLabels(ParameterId id)
        {
            if (_cacheParameterInfo.TryGetValue(id, out ImmutableArray<ParameterLabel> result))
            {
                return result;
            }

            result = ImmutableArray<ParameterLabel>.Empty;

            ImmutableArray<EventDescription> events = ListAllEvents();
            foreach (EventDescription e in events)
            {
                result = e.FetchLabelsForParameter(id);
                if (result.Length != 0)
                {
                    // Found it!
                    break;
                }
            }

            _cacheParameterInfo[id] = result;
            return result;
        }

        public ImmutableArray<ParameterId> ListAllLocalParameters()
        {
            if (_cacheLocalParameters is null)
            {
                var builder = ImmutableArray.CreateBuilder<ParameterId>();

                ImmutableArray<EventDescription> events = ListAllEvents();
                foreach (EventDescription e in events)
                {
                    builder.AddRange(e.FetchParameters(withoutFlags: ParameterFlags.Global));
                }

                _cacheLocalParameters = builder.ToImmutable();
            }

            return _cacheLocalParameters ?? ImmutableArray<ParameterId>.Empty;
        }
    }
}
