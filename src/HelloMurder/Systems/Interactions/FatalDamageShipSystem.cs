

using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Assets;
using HelloMurder.Components;
using HelloMurder.Services;
using Murder;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Particles;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace HelloMurder.Systems.Interactions
{
    [Filter(typeof(EnemyComponent))]
    [Messager(typeof(FatalDamageMessage))]
    internal class FatalDamageShipSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            // fx seems too much with bomb fx already playing
            //LibraryServices.Explode(0, world, entity.GetGlobalTransform().Vector2);

            // play sinking animation
            SpriteComponent spriteComponent = entity.GetSprite();
            
            entity.SetSprite(spriteComponent.PlayOnce("hit_sunk", false));

            entity.SetDestroyOnAnimationComplete(false);

            entity.SetVelocity(new Vector2(0f,20f));

            foreach(var c in entity.Children)
            {
                WorldParticleSystemTracker worldTracker = world.GetUnique<ParticleSystemWorldTrackerComponent>().Tracker;
                worldTracker.Deactivate(c);
                world.GetEntity(c).Unparent();
            }

            // Increase score
            var pointsValue = entity.GetScoreValue().Value;

            var scoreEntity = world.GetUniqueEntity<GameplayUIManagerComponent>();

            var scoreComponent = scoreEntity.GetGameplayUIManager();
            var newScore = scoreComponent.AddScore(pointsValue);

            scoreEntity.SetGameplayUIManager(newScore);

            HelloMurderSaveData save = SaveServices.GetOrCreateSave();

            save.LastAttemptScore = newScore.CurrentScore;
            if (newScore.CurrentScore > save.HighScore) 
            {
                save.HighScore = newScore.CurrentScore;
            }
            SaveServices.QuickSave();

            // Show floating score
            var position = entity.GetGlobalTransform().Vector2;
            float textWidth = Game.Data.GetFont(100).GetLineWidth(pointsValue.ToString());
            position.X -= textWidth / 2f;
            entity.SetShipScoreRender(DrawScore, pointsValue, position, 1.0f);
        }

        private void DrawScore(RenderContext render, Entity ship)
        {
            var shipScore = ship.GetShipScoreRender();

            var scoreText = shipScore.ScoreValue.ToString();

            var color = Color.White;
            color = color.FadeAlpha(shipScore.TextAlpha);
            var textDraw = new DrawInfo() { Sort = 0.4f, Color = color };

            var position = shipScore.ShipKilledPosition;

            RenderServices.DrawSimpleText(render.UiBatch,
                100,
                scoreText,
                position,
                textDraw);

            position.Y -= 50f * Game.DeltaTime;
            shipScore = new ShipScoreRenderComponent(DrawScore, shipScore.ScoreValue, position, shipScore.TextAlpha-Game.DeltaTime);
            ship.SetShipScoreRender(shipScore);
        }
    }
}
