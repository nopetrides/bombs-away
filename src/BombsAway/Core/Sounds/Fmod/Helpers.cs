using FMOD.Studio;
using Murder.Core.Sounds;
using Murder.Diagnostics;

namespace BombsAway.Core.Sounds.Fmod
{
    public static class FmodHelpers
    {
        internal static bool Check(FMOD.RESULT r, string? message = null)
        {
            if (r != FMOD.RESULT.OK)
            {
                GameLogger.Fail(message ?? "Error on fmod operation.");
                return false;
            }

            return true;
        }

        internal static FMOD.GUID ToFmodGuid(this SoundEventId id) =>
            new FMOD.GUID { Data1 = id.Data1, Data2 = id.Data2, Data3 = id.Data3, Data4 = id.Data4 };

        internal static SoundEventId ToSoundId(this FMOD.GUID id) =>
            new SoundEventId { Data1 = id.Data1, Data2 = id.Data2, Data3 = id.Data3, Data4 = id.Data4 };

        internal static FMOD.Studio.PARAMETER_ID ToFmodId(this ParameterId id) =>
            new FMOD.Studio.PARAMETER_ID { Data1 = id.Data1, Data2 = id.Data2 };

        public static ParameterId ToParameterId(this FMOD.Studio.PARAMETER_DESCRIPTION description, SoundEventId? owner = null) =>
            new ParameterId { Data1 = description.Id.Data1, Data2 = description.Id.Data2, Name = description.Name, Owner = owner, IsGlobal = description.Flags.HasFlag(PARAMETER_FLAGS.GLOBAL) };

        public static bool HasParameterFlag(this PARAMETER_FLAGS input, ParameterFlags flags)
        {
            return ((int)input & (int)flags) == (int)flags;
        }

        public static bool HasAnyParameterFlag(this PARAMETER_FLAGS input, ParameterFlags flags)
        {
            return ((int)input & (int)flags) != 0;
        }
    }
}
