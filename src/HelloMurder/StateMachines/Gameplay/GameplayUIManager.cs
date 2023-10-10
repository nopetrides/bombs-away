using Bang.Entities;
using Bang.StateMachines;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Newtonsoft.Json;
using Murder.Services;
using HelloMurder.Components;

namespace HelloMurder.StateMachines.Gameplay
{
    public class GameplayUIManager : StateMachine
    {
        [JsonProperty, GameAssetId<SpriteAsset>]
        private readonly Guid _scoreBG = Guid.Empty;
        [JsonProperty]
        private readonly float _uiRectWidth = 100f;
        [JsonProperty]
        private readonly float _uiRectHeight = 15f;

        private GameplayUIManagerComponent _managerComponent;

        public GameplayUIManager()
        {
            State(Level);
        }

        private IEnumerator<Wait> Level()
        {
            Entity.SetGameplayUIManager();
            _managerComponent = Entity.GetGameplayUIManager();
            Entity.SetCustomDraw(DrawScoreUI);
            yield return Wait.NextFrame;
        }

        private void DrawScoreUI(RenderContext render)
        {
            var world = (MonoWorld)World;
            var bound = world.Camera.Bounds;
            var bgRect = new Rectangle(
                    (bound.X + (bound.Width / 2f)) - (_uiRectWidth / 2f),
                    bound.Y,
                    _uiRectWidth,
                    _uiRectHeight);
            RenderServices.Draw9Slice(render.GameUiBatch,
                _scoreBG,
                bgRect,
                new DrawInfo() { Sort = 0.5f });

            var scoreText = "Score: " + _managerComponent.CurrentScore;
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = Color.Black };
            var position = bgRect.CenterLeft;
            position.X += 6;
            position.Y -= 2;
            RenderServices.DrawSimpleText(render.GameUiBatch, 
                100, 
                scoreText,
                position,
                textDraw);
        }
    }
}
