

using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Assets;
using HelloMurder.Components;
using HelloMurder.Services;
using Murder.Messages;
using Murder.Utilities;

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
            Murder.Components.SpriteComponent spriteComponent = entity.GetSprite();
            
            entity.SetSprite(spriteComponent.PlayOnce("hit_sunk", false));

            entity.SetDestroyOnAnimationComplete(false);

            // Increase score
            var pointsValue = entity.GetScoreValue().Value;

            var scoreEntity = world.GetUniqueEntity<GameplayUIManagerComponent>();

            var scoreComponent = scoreEntity.GetGameplayUIManager();
            var newScore = scoreComponent.AddScore(pointsValue);

            scoreEntity.SetGameplayUIManager(newScore);

            HelloMurderSaveData save = SaveServices.GetOrCreateSave();

            if (newScore.CurrentScore > save.HighScore) 
            {
                save.HighScore = newScore.CurrentScore;

                SaveServices.QuickSave();
            }
        }
    }
}
