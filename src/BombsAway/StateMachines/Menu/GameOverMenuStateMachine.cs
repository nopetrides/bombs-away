using Bang.StateMachines;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Input;
using Murder.Services;
using Newtonsoft.Json;
using Bang.Entities;
using Murder.Core.Graphics;
using Murder;
using BombsAway.Core;
using System.Diagnostics;
using Murder.Core.Geometry;
using BombsAway.Services;
using System.Numerics;
using BombsAway.Assets;
using BombsAway.Core.Sounds;
using Murder.Utilities;
using Murder.Core;

namespace BombsAway.StateMachines.Menu
{
    internal class GameOverMenuStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _mainMenuWorld = Guid.Empty;

        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _gameWorld = Guid.Empty;

        [JsonProperty]
        private readonly float _paperDrawDelay = 5f;
        [JsonProperty]
        private readonly float _paperRestartDelay = 15f;

        private MenuInfo GetGameOverOptions() =>
            new MenuInfo(options: new MenuOption[] { new("Restart"), new("Quit") });

        private MenuInfo _menuInfo = new();

        private float _paperStartTimer = 0f;
        private float _paperAnimationTimer = 0f;

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

            BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().EndScreen, Murder.Core.Sounds.SoundProperties.StopOtherMusic);

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

            RenderServices.DrawSprite(render.GameUiBatch, skin,
                new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f), 
                new DrawInfo(1f)
                {
                    Origin = new Vector2(.5f, .5f)
                });

            if (_paperStartTimer > _paperDrawDelay)
            {
                if (_paperAnimationTimer == 0f)
                {
                    var paperGuid = LibraryServices.GetLibrary().GameOverPaper;
                    var paper = AssetServices.TryCreate(World, paperGuid);
                    var world = (MonoWorld)World;
                    paper?.SetGlobalPosition(world.Camera.Position);
                    paper?.SetDestroyOnAnimationComplete(false);
                    // Tried to determine how to enable and disable this entity instead of destroying, but oh well
                    _paperAnimationTimer += Game.DeltaTime;
                }
                else if (_paperAnimationTimer > _paperRestartDelay)
                {
                    _paperAnimationTimer = 0f;
                }
                else
                {
                    _paperAnimationTimer += Game.DeltaTime;
                }
            }
            else
            {
                _paperStartTimer += Game.DeltaTime;
            }

            // Menu options
            Point cameraHalfSize = render.Camera.Size / 2f - new Point(0, _menuInfo.Length * 18);

            RenderServices.DrawVerticalMenu(
                render.UiBatch,
                cameraHalfSize,
                new DrawMenuStyle()
                {
                    Color = Palette.Colors[7],
                    Shadow = Palette.Colors[1],
                    SelectedColor = Palette.Colors[2],
                    Origin = new(0.5f, 0.5f),
                    ExtraVerticalSpace = 19,
                },
                _menuInfo);


            BombsAwaySaveData save = SaveServices.GetOrCreateSave();
            // Score
            DrawScore(render, save);
            // High score
            DrawHighScore(render, save);
            //Credits
            DrawCredits(render);
        }


        private void DrawScore(RenderContext render, BombsAwaySaveData save)
        {
            var score = save.LastAttemptScore;
            var scoreText = "Score: " + score;
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f);
            position.X -= 100;
            position.Y += 8;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                scoreText,
                position,
                textDraw);

        }
        private void DrawHighScore(RenderContext render, BombsAwaySaveData save)
        {
            var highScore = save.HighScore;
            var highScoreText = "High Score: " + highScore;
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f);
            position.X += 50;
            position.Y += 8;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                highScoreText,
                position,
                textDraw);
        }

        private void DrawCredits(RenderContext render)
        {
            var creditsText =
@"                                                          Bombs Away

                                                              Made By:
                                                         Noah Petrides
                                                                     &
                                                           Alex Belland

               Created using Murder Engine by isadorasophia and saint11
";
            var fmodAttribution = "Audio Engine courtesy of FMOD Studio by Firelight Technologies Pty Ltd.";

            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f);
            var lineWidth = Game.Data.GetFont(100).GetLineWidth(fmodAttribution);
            creditsText += fmodAttribution;
            position.X -= lineWidth/2f;
            position.Y -= 190;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                creditsText,
                position,
                textDraw);
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();
        }
    }
}
