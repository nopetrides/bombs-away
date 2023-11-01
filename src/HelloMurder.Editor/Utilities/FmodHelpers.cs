using HelloMurder.Core.Sounds;
using Murder;
using Murder.Core.Sounds;

namespace HelloMurder.Editor.Utilities
{
    public static class FmodHelpers
    {
        public static bool IsValidEvent(SoundEventId sound)
        {
            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return false;
            }

            return player.HasEvent(sound);
        }

        public static bool IsValidBus(SoundEventId sound)
        {
            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return false;
            }

            return player.HasBus(sound);
        }

        public static bool IsValidParameter(ParameterId parameter)
        {
            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return false;
            }

            return player.FetchParameterDescription(parameter) is not null;
        }
    }
}
