using FMOD.Studio;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace BombsAway.Core.Sounds.Fmod
{
    /// <summary>
    /// Wrapper for an event. This is used to create event instances.
    /// This can be reetrieved from a bank by querying all events in a bank, or through
    /// a specific path or guid.
    /// </summary>
    public class EventDescription : IDisposable
    {
        private readonly FMOD.Studio.EventDescription _event;

        private string? _path;
        private FMOD.GUID? _id;

        private ImmutableArray<EventInstance>? _instancesCached;

        public bool IsLoaded { get; private set; }

        public EventDescription(FMOD.Studio.EventDescription @event)
        {
            _event = @event;
        }

        public SoundEventId Id => Guid.ToSoundId().WithPath(Path);

        public string Path
        {
            get
            {
                if (_path is null)
                {
                    _event.GetPath(out string? path);
                    _path = path;
                }

                return _path!;
            }
        }

        public FMOD.GUID Guid
        {
            get
            {
                if (_id is null)
                {
                    _event.GetID(out FMOD.GUID id);
                    _id = id;
                }

                return _id.Value;
            }
        }

        public ImmutableArray<EventInstance> GetInstances()
        {
            if (_instancesCached is null)
            {
                FMOD.RESULT result = _event.GetInstanceList(out FMOD.Studio.EventInstance[]? instances);
                if (result != FMOD.RESULT.OK || instances is null)
                {
                    GameLogger.Error("Unable to list instances for an event.");
                    return ImmutableArray<EventInstance>.Empty;
                }

                _instancesCached = instances.Select(e => new EventInstance(e)).ToImmutableArray();
            }

            return _instancesCached.Value;
        }

        /// <summary>
        /// Create a new instance of the event.
        /// Loading sample data through this method will require a bit of time,
        /// make sure you don't need to play the event immediately.
        /// </summary>
        public EventInstance CreateInstance()
        {
            _event.CreateInstance(out FMOD.Studio.EventInstance instance);
            _instancesCached = null;

            return new(instance);
        }

        /// <summary>
        /// There are three ways to load sample data (any non-streamed sound):
        ///   - From a bank. This will keep all the bank's data in memory, until unloaded.
        ///   - From an event description. This will keep the event's encessary data in memory until unloaded.
        ///   - From an event instance. The data will only be in memory while the instance is.
        /// </summary>
        public void LoadSampleData()
        {
            FMOD.RESULT result = _event.LoadSampleData();
            GameLogger.Verify(result == FMOD.RESULT.OK, "Unable to load sample data for target event.");

            IsLoaded = true;
        }

        public PARAMETER_DESCRIPTION? FetchParameterDescription(ParameterId parameter)
        {
            FMOD.RESULT result = _event.GetParameterDescriptionByID(parameter.ToFmodId(), out PARAMETER_DESCRIPTION description);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return description;
        }

        /// <summary>
        /// List all the labels for a specific parameter.
        /// </summary>
        public ImmutableArray<ParameterLabel> FetchLabelsForParameter(ParameterId parameter)
        {
            FMOD.RESULT result = _event.GetParameterDescriptionByID(parameter.ToFmodId(), out PARAMETER_DESCRIPTION description);
            if (result != FMOD.RESULT.OK)
            {
                return ImmutableArray<ParameterLabel>.Empty;
            }

            if (!description.Flags.HasFlag(PARAMETER_FLAGS.LABELED))
            {
                // Not a labeled parameter, dismiss any inspection.
                return ImmutableArray<ParameterLabel>.Empty;
            }

            float maximum = description.Maximum;
            float minimum = description.Minimum;
            if (minimum >= maximum)
            {
                // Invalid?
                return ImmutableArray<ParameterLabel>.Empty;
            }

            var builder = ImmutableArray.CreateBuilder<ParameterLabel>();

            float value = Calculator.RoundToInt(minimum);
            int index = 0;
            while (value <= maximum)
            {
                if (FMOD.RESULT.OK == _event.GetParameterLabelByID(description.Id, index, out string? label) &&
                    label is not null)
                {
                    builder.Add(new(label, value));
                }

                value++;
                index++;
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// List all the parameters available in the bank.
        /// </summary>
        public ImmutableArray<ParameterId> FetchParameters(
        ParameterFlags withFlags = ParameterFlags.None,
            ParameterFlags withoutFlags = ParameterFlags.None)
        {
            FMOD.RESULT result = _event.GetParameterDescriptionCount(out int count);
            if (result != FMOD.RESULT.OK || count == 0)
            {
                GameLogger.Verify(result == FMOD.RESULT.OK, "Unable to list parameters for studio instance.");
                return ImmutableArray<ParameterId>.Empty;
            }

            var parameters = ImmutableArray.CreateBuilder<ParameterId>(count);
            for (int i = 0; i < count; ++i)
            {
                result = _event.GetParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION parameter);
                if (result != FMOD.RESULT.OK)
                {
                    GameLogger.Fail("Unable to fetch parameter description for event.");
                    return ImmutableArray<ParameterId>.Empty;
                }

                // Apply filters according to flags.
                if (withFlags != ParameterFlags.None && !parameter.Flags.HasParameterFlag(withFlags))
                {
                    continue;
                }

                if (withoutFlags != ParameterFlags.None && parameter.Flags.HasAnyParameterFlag(withoutFlags))
                {
                    continue;
                }

                parameters.Add(parameter.ToParameterId(owner: Id));
            }



            return parameters.ToImmutable();
        }

        public void Dispose()
        {
            _event.UnloadSampleData();
            _event.ReleaseAllInstances();
        }
    }
}
