using Murder;
using Murder.Save;
using Newtonsoft.Json;

namespace BombsAway.Data
{
    public class BombsAwayPreferences : GamePreferences
    {
        public BombsAwayPreferences() : base() { }

        [JsonProperty]
        protected int _highScore = 0;

        public override void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(id: BombsAwayGame.Profile.MusicBus, _musicVolume);
            Game.Sound.SetVolume(id: BombsAwayGame.Profile.SoundBus, _soundVolume);
        }

        public int HighScore => _highScore;

        /// <summary>
        /// Preferences is currently acting like game save. Game save is not retaining data between sessions
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SetHighScore(int value)
        {
            _highScore = value;

            OnPreferencesChanged();
            return _highScore;
        }
    }
}
