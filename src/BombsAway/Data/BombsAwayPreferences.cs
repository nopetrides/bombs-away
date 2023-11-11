using Murder;
using Murder.Save;

namespace BombsAway.Data
{
    public class BombsAwayPreferences : GamePreferences
    {
        public BombsAwayPreferences() : base() { }

        public override void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(id: BombsAwayGame.Profile.MusicBus, _musicVolume);
            Game.Sound.SetVolume(id: BombsAwayGame.Profile.SoundBus, _soundVolume);
        }
    }
}
