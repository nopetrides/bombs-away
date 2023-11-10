using HelloMurder.Assets;
using HelloMurder.Core;
using HelloMurder.Core.Sounds;
using HelloMurder.Data;
using Microsoft.Xna.Framework.Input;
using Murder;
using Murder.Assets;
using Murder.Core.Input;
using Murder.Core.Sounds;
using Murder.Save;
using System.Collections.Immutable;

namespace HelloMurder
{
    public class HelloMurderGame : IMurderGame
    {
        public static HelloMurderProfile Profile => (HelloMurderProfile)Game.Profile;
        public string Name => "HelloMurder";
        public GameProfile CreateGameProfile() => new HelloMurderProfile();
        public SaveData CreateSaveData(string name) => new HelloMurderSaveData(name);

        public GamePreferences CreateGamePreferences() => new HelloMurderPreferences();

        public ISoundPlayer CreateSoundPlayer() => new HelloMurderSoundPlayer();

        public void Initialize()
        {
            // Just some values to play around with rendering and window
            //var a = Game.Instance.Window;
            //var b = Game.GraphicsDevice.DisplayMode.AspectRatio;

            Game.Data.CurrentPalette = Palette.Colors.ToImmutableArray();

            // Registers Movement Axis Input
            GamepadAxis[] stick =
            {
                GamepadAxis.LeftThumb,
                GamepadAxis.Dpad
            };

            // Registers movement from left stick or dpad
            Game.Input.RegisterAxes(MurderInputAxis.Movement, stick);

            // Registeres movement from wasd and arrow keys
            Game.Input.Register(MurderInputAxis.Movement,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new InputButtonAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            // Registers movement for the UI with wasd and arrow keys
            Game.Input.Register(MurderInputAxis.Ui,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new InputButtonAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            Game.Input.Register(InputButtons.Attack, Keys.Z);
            Game.Input.Register(InputButtons.Attack, Buttons.X);
            Game.Input.Register(InputButtons.Attack, Keys.Space);
        }
    }
}
