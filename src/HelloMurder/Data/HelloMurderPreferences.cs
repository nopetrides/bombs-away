using Murder;
using Murder.Save;

namespace HelloMurder.Data
{
    public class HelloMurderPreferences : GamePreferences
    {
        public HelloMurderPreferences() : base() { }

        public override void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(id: HelloMurderGame.Profile.MusicBus, _musicVolume);
            Game.Sound.SetVolume(id: HelloMurderGame.Profile.SoundBus, _soundVolume);
        }
    }
}
