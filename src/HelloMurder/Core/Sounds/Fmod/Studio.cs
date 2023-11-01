using FMOD.Studio;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace HelloMurder.Core.Sounds.Fmod
{
    internal class Studio : IDisposable
    {
        private readonly FMOD.Studio.System _studio;

        public Studio(FMOD.Studio.System studio)
        {
            _studio = studio;
        }

        /// <summary>
        /// Load a fmod bank asynchronously from a file path specified by <paramref name="path"/>.
        /// </summary>
        public async ValueTask<Bank> LoadBankAsync(
            string path,
            LOAD_BANK_FLAGS flags = LOAD_BANK_FLAGS.NORMAL)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);

            FMOD.RESULT result = _studio.LoadBankMemory(bytes, flags, out FMOD.Studio.Bank bank);
            FmodHelpers.Check(result, $"Unable to load bank from memory for {path}.");

            return new(bank, Path.GetFileNameWithoutExtension(path));
        }

        public void Update()
        {
            _studio.Update();
        }

        /// <summary>
        /// List all the parameters available in the bank.
        /// </summary>
        public ImmutableArray<ParameterId> FetchParameters()
        {
            FMOD.RESULT result = _studio.GetParameterDescriptionList(out PARAMETER_DESCRIPTION[]? parameters);
            if (result != FMOD.RESULT.OK || parameters is null)
            {
                GameLogger.Error("Unable to list parameters for studio instance.");
                return ImmutableArray<ParameterId>.Empty;
            }

            return parameters.Select(r => r.ToParameterId()).ToImmutableArray();
        }

        public PARAMETER_DESCRIPTION? FetchParameterDescription(ParameterId parameter)
        {
            FMOD.RESULT result = _studio.GetParameterDescriptionByID(parameter.ToFmodId(), out PARAMETER_DESCRIPTION description);
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
            FMOD.RESULT result = _studio.GetParameterDescriptionByID(parameter.ToFmodId(), out PARAMETER_DESCRIPTION description);
            if (result != FMOD.RESULT.OK)
            {
                return ImmutableArray<ParameterLabel>.Empty;
            }

            if (description.Flags != PARAMETER_FLAGS.LABELED)
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
                if (FMOD.RESULT.OK == _studio.GetParameterLabelByID(description.Id, index, out string? label) &&
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
        /// This fetches an event based on an internal guid represeted by <paramref name="id"/>.
        /// </summary>
        public EventDescription? GetEvent(SoundEventId id)
        {
            FMOD.GUID guid = id.ToFmodGuid();

            FMOD.RESULT result = _studio.GetEventByID(guid, out FMOD.Studio.EventDescription description);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(description);
        }

        /// <summary>
        /// This fetches an event based on an internal path, i.e. "event:/UI/Cancel".
        /// </summary>
        public EventDescription? GetEvent(string path)
        {
            FMOD.RESULT result = _studio.GetEvent(path, out FMOD.Studio.EventDescription description);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(description);
        }

        /// <summary>
        /// This fetches a bus based on an internal path, i.e. "bus:/UI".
        /// </summary>
        public Bus? GetBus(SoundEventId id)
        {
            FMOD.RESULT result = _studio.GetBusByID(id.ToFmodGuid(), out FMOD.Studio.Bus bus);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(bus);
        }

        /// <summary>
        /// This fetches a bus based on an internal path, i.e. "bus:/UI".
        /// </summary>
        public Bus? GetBus(string path)
        {
            FMOD.RESULT result = _studio.GetBus(path, out FMOD.Studio.Bus bus);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(bus);
        }

        /// <summary>
        /// Retrieves a global parameter target value.
        /// This *ignores* modulation or automation applied to the parameter within the Studio.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        public float GetParameterTargetValue(ParameterId id)
        {
            FMOD.RESULT result = _studio.GetParameterByID(id.ToFmodId(), out float _, out float finalValue);
            if (result != FMOD.RESULT.OK)
            {
                return -1;
            }

            return finalValue;
        }

        /// <summary>
        /// Retrieves a global parameter current value.
		/// This takes into account modulation / automation applied to the parameter within Studio.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        public float GetParameterCurrentValue(ParameterId id)
        {
            FMOD.RESULT result = _studio.GetParameterByID(id.ToFmodId(), out float value, out float _);
            if (result != FMOD.RESULT.OK)
            {
                return -1;
            }

            return value;
        }

        /// <summary>
        /// Set a global parameter value according to its name.
        /// </summary>
        /// <param name="name">Name of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public bool SetParameterValue(string name, float value, bool ignoreSeekSpeed = false)
        {
            FMOD.RESULT result = _studio.SetParameterByName(name, value, ignoreSeekSpeed);
            return result == FMOD.RESULT.OK;
        }

        /// <summary>
        /// Set a global parameter value according to its id.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public bool SetParameterValue(ParameterId id, float value, bool ignoreSeekSpeed = false)
        {
            FMOD.RESULT result = _studio.SetParameterByID(id.ToFmodId(), value, ignoreSeekSpeed);
            return result == FMOD.RESULT.OK;
        }

        /// <summary>
        /// Set a collection of global parameters according to its id.
        /// </summary>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        /// <param name="parameters">Collection of parameters.</param>
        public void SetParameterValues(bool ignoreSeekSpeed, params (ParameterId id, float value)[] parameters)
        {
            PARAMETER_ID[] ids = parameters.Select(i => i.id.ToFmodId()).ToArray();
            float[] values = parameters.Select(i => i.value).ToArray();

            _studio.SetParametersByIDs(ids, values, values.Length, ignoreSeekSpeed);
        }

        public void Dispose()
        {
            _studio.Release();
        }
    }
}
