using Bang.Entities;
using Bang.StateMachines;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Newtonsoft.Json;
using Murder.Services;
using HelloMurder.Assets;
using HelloMurder.Services;

namespace HelloMurder.StateMachines.Gameplay
{
    public class GameplayUIManager : StateMachine
    {
        [JsonProperty, GameAssetId<SpriteAsset>]
        private readonly Guid _scoreBG = Guid.Empty;
        [JsonProperty]
        private readonly float _uiRectWidth = 100f;
        [JsonProperty]
        private readonly float _uiRectHeight = 16f;

        public GameplayUIManager()
        {
            State(Level);
        }

        private IEnumerator<Wait> Level()
        {
            Entity.SetGameplayUIManager();
            Entity.SetCustomDraw(DrawScoreUI);
            yield return Wait.NextFrame;
        }

        private void DrawScoreUI(RenderContext render)
        {
            var world = (MonoWorld)World;
            var bounds = world.Camera.Bounds;

            // Score
            DrawScore(render, bounds);
            // High score
            DrawHighScore(render, bounds);

        }

        private void DrawScore(RenderContext render, Rectangle bounds)
        {
            var scoreBgRect = new Rectangle(
                    bounds.X+3,
                    bounds.Y,
                    _uiRectWidth,
                    _uiRectHeight);
            RenderServices.Draw9Slice(render.UiBatch,
                _scoreBG,
                scoreBgRect,
                new DrawInfo() { Sort = 0.5f });

            var score = Entity.GetGameplayUIManager().CurrentScore;
            var scoreText = "Score: " + score;
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = scoreBgRect.CenterLeft;
            position.X += 6;
            position.Y -= 2;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                scoreText,
                position,
                textDraw);

        }
        private void DrawHighScore(RenderContext render, Rectangle bounds)
        {
            var highscoreBgRect = new Rectangle(
            (bounds.Width - _uiRectWidth),
               bounds.Y,
        _uiRectWidth,
        _uiRectHeight);
            RenderServices.Draw9Slice(render.UiBatch,
                _scoreBG,
                highscoreBgRect,
                new DrawInfo() { Sort = 0.5f });
            HelloMurderSaveData save = SaveServices.GetOrCreateSave();
            var highScore = save.HighScore;
            var highScoreText = "High Score: " + highScore;
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = highscoreBgRect.CenterLeft;
            position.X += 6;
            position.Y -= 2;
            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                highScoreText,
                position,
                textDraw);
        }
    }
}
