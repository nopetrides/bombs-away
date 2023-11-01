using FMOD.Studio;

namespace HelloMurder.Core.Sounds.Fmod
{
    /// <summary>
    /// Wrapper for an event instance.
    /// An event can theoretically have any number of these, but only one event description.
    /// </summary>
    public class EventInstance : IDisposable
    {
        private readonly FMOD.Studio.EventInstance _instance;

        public EventInstance(FMOD.Studio.EventInstance instance)
        {
            _instance = instance;
        }

        public bool Start()
        {
            FMOD.RESULT result = _instance.Start();
            if (result != FMOD.RESULT.OK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stop the event instance.
        /// </summary>
        /// <param name="isFadeOut">If true, this applies a fade out to the sound.</param>
        public bool Stop(bool isFadeOut)
        {
            FMOD.RESULT result = _instance.Stop(isFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            if (result != FMOD.RESULT.OK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set the instance parameter value according to its name.
        /// </summary>
        /// <param name="name">Name of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public bool SetParameterValue(string name, float value, bool ignoreSeekSpeed = false)
        {
            FMOD.RESULT result = _instance.SetParameterByName(name, value, ignoreSeekSpeed);
            if (result != FMOD.RESULT.OK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set the instance parameter value according to its id.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public bool SetParameterValue(PARAMETER_ID id, float value, bool ignoreSeekSpeed = false)
        {
            FMOD.RESULT result = _instance.SetParameterByID(id, value, ignoreSeekSpeed);
            if (result != FMOD.RESULT.OK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set a collection of instance parameters according to its id.
        /// </summary>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        /// <param name="parameters">Collection of parameters.</param>
        public bool SetParameterValues(bool ignoreSeekSpeed, params (PARAMETER_ID id, float value)[] parameters)
        {
            PARAMETER_ID[] ids = parameters.Select(i => i.id).ToArray();
            float[] values = parameters.Select(i => i.value).ToArray();

            FMOD.RESULT result = _instance.SetParametersByIDs(ids, values, values.Length, ignoreSeekSpeed);
            if (result != FMOD.RESULT.OK)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _instance.Release();
        }
    }
}
