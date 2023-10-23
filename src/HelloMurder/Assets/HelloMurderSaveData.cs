using Murder.Assets;

namespace HelloMurder.Assets
{
    public class HelloMurderSaveData : Murder.Assets.SaveData
    {
        public int HighScore = 0;

        public HelloMurderSaveData(string name) : base(name) { }
    }
}
