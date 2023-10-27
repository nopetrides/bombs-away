using Bang.StateMachines;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Input;
using Murder.Services;
using Newtonsoft.Json;
using Bang.Entities;
using Murder.Core.Graphics;
using Murder;
using HelloMurder.Core;
using System.Diagnostics;
using Murder.Core.Geometry;
using HelloMurder.Services;
using System.Numerics;

namespace HelloMurder.StateMachines.Menu
{
    internal class GameOverMenuStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _mainMenuWorld = Guid.Empty;

        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _gameWorld = Guid.Empty;

        private MenuInfo GetGameOverOptions() =>
            new MenuInfo(options: new MenuOption[] { new("Restart"), new("Quit") });

        private MenuInfo _menuInfo = new();

        public GameOverMenuStateMachine()
        {
            State(GameOver);
        }

        private IEnumerator<Wait> GameOver()
        {
            Entity.SetCustomDraw(DrawGameOverMenu);
            yield return GoTo(Main);
        }

        private IEnumerator<Wait> Main()
        {
            _menuInfo = GetGameOverOptions();
            _menuInfo.Select(_menuInfo.NextAvailableOption(-1, 1));

            while (true)
            {
                if (Game.Input.VerticalMenu(ref _menuInfo))
                {
                    switch (_menuInfo.Selection)
                    {
                        case 0: //  Restart
                            Game.Instance.QueueWorldTransition(_gameWorld);
                            break;

                        case 1: //  Quit

                            Game.Instance.QueueWorldTransition(_mainMenuWorld);
                            break;

                        default:
                            break;
                    }
                }

                yield return Wait.NextFrame;
            }
        }

        private void DrawGameOverMenu(RenderContext render)
        {
            Debug.Assert(_menuInfo.Options is not null);

            // BG Image
            var skin = LibraryServices.GetLibrary().GameOverScreen;

            RenderServices.DrawSprite(render.UiBatch, skin,
                new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f), new DrawInfo(0.8f)
                {
                    Origin = new Vector2(.5f, .5f)
                });

            // Menu options
            Point cameraHalfSize = render.Camera.Size / 2f - new Point(0, _menuInfo.Length * 7);

            RenderServices.DrawVerticalMenu(
                render.UiBatch,
                cameraHalfSize,
                new DrawMenuStyle()
                {
                    Color = Palette.Colors[7],
                    Shadow = Palette.Colors[1],
                    SelectedColor = Palette.Colors[9],
                    Origin = new(0.5f, 0.5f),
                    ExtraVerticalSpace = 19,
                },
                _menuInfo);
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();
        }
    }
}
