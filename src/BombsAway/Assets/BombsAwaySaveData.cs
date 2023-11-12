using Murder.Assets;

namespace BombsAway.Assets
{
    public class BombsAwaySaveData : Murder.Assets.SaveData
    {
        /// <summary>
        /// Not correctly working in Save Data currently. See <see cref="Data.BombsAwayPreferences"/>
        /// </summary>
        public int HighScore = 0;

        public int LastAttemptScore = 0;

        public BombsAwaySaveData(string name) : base(name) { }
    }
}
