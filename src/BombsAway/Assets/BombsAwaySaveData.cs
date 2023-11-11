using Murder.Assets;

namespace BombsAway.Assets
{
    public class BombsAwaySaveData : Murder.Assets.SaveData
    {
        public int HighScore = 0;

        public int LastAttemptScore = 0;

        public BombsAwaySaveData(string name) : base(name) { }
    }
}
