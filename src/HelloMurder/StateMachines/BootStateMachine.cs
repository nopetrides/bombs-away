using Bang.Entities;
using Bang.StateMachines;
using HelloMurder.Assets;
using HelloMurder.Core;
using HelloMurder.Services;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Services;
using Murder;
using System.Numerics;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using HelloMurder.Core.Sounds;


namespace HelloMurder.StateMachines
{
    internal class BootStateMachine : StateMachine
    {
        private bool _showFmod = true;

        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private readonly Guid _nextWorld = Guid.Empty;

        private float _alpha = 0f;

        public BootStateMachine()
        {
            State(Main);
        }

        private float _lastTransition = 0;

        private IEnumerator<Wait> Main()
        {
            Entity.SetCustomDraw(Draw);

            HelloMurderSoundPlayer.Instance.Stop(null, false);

            HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().MainMenuMusic, Murder.Core.Sounds.SoundProperties.StopOtherMusic);

            // fade in
            float duration = 0.5f;
            _lastTransition = Game.Now;
            while (Game.Now - _lastTransition < duration)
            {
                _alpha = (Game.Now - _lastTransition) / duration;
                yield return Wait.NextFrame;
            }

            _alpha = 1.0f;

            // wait, skippable
            duration = 2f;
            _lastTransition = Game.Now;
            while (Game.Now - _lastTransition < duration && !Game.Input.PressedAndConsume(InputButtons.Attack) && !Game.Input.PressedAndConsume(InputButtons.Cancel))
            {
                yield return Wait.NextFrame;
            }


            // fade out
            duration = 0.5f;
            _lastTransition = Game.Now;
            while (Game.Now - _lastTransition < duration)
            {
                _alpha = 1 - ((Game.Now - _lastTransition) / duration);
                yield return Wait.NextFrame;
            }
            _alpha = 0.0f;

            yield return Wait.NextFrame;

            LevelServices.SwitchScene(_nextWorld);
        }

        private void Draw(RenderContext render)
        {
            LibraryAsset library = LibraryServices.GetLibrary();

            var murder = library.MurderLogo;

            // Murder Logo
            Color logoBlack = Color.Black;
            logoBlack = logoBlack.FadeAlpha(1 - _alpha);

            RenderServices.DrawSprite(render.GameUiBatch, murder,
                new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f - 200f), 
                new DrawInfo(logoBlack, 0.8f) { Origin = new Vector2(.5f, .5f), BlendMode = BlendStyle.Wash});

            // Credits text
            Color textWhite = Color.White;
            textWhite = textWhite.FadeAlpha(_alpha);
            var creditsText =
@"   A Game By:
Noah Petrides
            &
  Alex Belland";

            var textDraw = new DrawInfo(textWhite, 0.4f);
            var position = new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f);
            var lineWidth = Game.Data.GetFont(100).GetLineWidth(creditsText);
            position.X -= lineWidth / 2f;
            position.Y -= 30;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                creditsText,
                position,
                textDraw);

            if (_showFmod)
            {
                var fmod = library.FmodLogo;
                RenderServices.DrawSprite(render.GameUiBatch, fmod,
                    new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f + 200f), 
                    new DrawInfo(logoBlack, 0.8f) { Origin = new Vector2(.5f, .5f), BlendMode = BlendStyle.Wash });
            }
        }
    }
}
