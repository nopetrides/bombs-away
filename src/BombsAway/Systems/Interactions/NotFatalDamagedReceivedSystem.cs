using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using BombsAway.Components;
using BombsAway.Messages;
using BombsAway.Services;
using Murder.Utilities;

namespace BombsAway.Systems.Interactions
{
    [Filter(typeof(EnemyComponent))]
    [Messager(typeof(HealthDamagedMessage))]
    internal class NotFatalDamagedReceivedSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            // bomb already plays an fx
            //LibraryServices.Explode(0, world, entity.GetGlobalTransform().Vector2);

            Murder.Components.SpriteComponent spriteComponent = entity.GetSprite();

            var health = entity.GetHealth().Health;
            string animation = "float_damaged" + health;
            entity.SetSprite(spriteComponent.Play(false, animation));
            var child = entity.TryFetchChild("damaged" + health);
            child?.Activate();
        }
    }
}
