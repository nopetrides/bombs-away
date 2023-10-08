using HelloMurder.Assets;
using HelloMurder.Core;
using Microsoft.Xna.Framework.Input;
using Murder;
using Murder.Core.Input;
using System.Collections.Immutable;

namespace HelloMurder
{
    public class HelloMurderGame : IMurderGame
    {
        public static HelloMurderProfile Profile => (HelloMurderProfile)Game.Profile;
        public string Name => "HelloMurder";

        public void Initialize()
        {
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

            // Registers movement for UI tabs with QE and PuPd
            Game.Input.Register(MurderInputAxis.UiTab,
                new InputButtonAxis(Keys.Q, Keys.Q, Keys.E, Keys.E),
                new InputButtonAxis(Keys.PageUp, Keys.PageUp, Keys.PageDown, Keys.PageDown));

            // Registers moveent for UI tabs with shoulder buttons
            Game.Input.Register(MurderInputAxis.UiTab,
                new InputButtonAxis(Buttons.LeftShoulder, Buttons.LeftShoulder, Buttons.RightShoulder, Buttons.RightShoulder));

            Game.Input.Register(InputButtons.LockMovement, Keys.LeftShift, Keys.RightShift);
            Game.Input.Register(InputButtons.LockMovement, Buttons.LeftTrigger, Buttons.RightTrigger);

            Game.Input.Register(InputButtons.Attack, Keys.Z);
            Game.Input.Register(InputButtons.Attack, Buttons.X);

            Game.Input.Register(InputButtons.Submit, Keys.Enter, Keys.Space);
            Game.Input.Register(InputButtons.SubmitWithEnter, Keys.Enter);

            Game.Input.Register(InputButtons.Cancel, Keys.Escape, Keys.Delete, Keys.Back, Keys.BrowserBack);

            Game.Input.Register(InputButtons.Interact, Keys.Space);
            Game.Input.Register(InputButtons.Interact, Buttons.Y);
            
            Game.Input.Register(InputButtons.Skip, Keys.Back, Keys.Escape, Keys.O);
            Game.Input.Register(InputButtons.Skip, Buttons.Start);
            
            Game.Input.Register(MurderInputButtons.Pause, Keys.Escape, Keys.P);
            Game.Input.Register(MurderInputButtons.Pause, Buttons.Start);
        }
    }
}
