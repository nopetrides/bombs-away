using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Messages;
using HelloMurder.Services;
using Murder.Utilities;

namespace HelloMurder.Systems.Interactions
{
    [Filter(typeof(EnemyComponent))]
    [Messager(typeof(HealthDamagedMessage))]
    internal class NotFatalDamagedReceivedSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            LibraryServices.Explode(0, world, entity.GetGlobalTransform().Vector2);

            Murder.Components.SpriteComponent spriteComponent = entity.GetSprite();

            var health = entity.GetHealth().Health;
            string animation = "float_damaged" + health;
            entity.SetSprite(spriteComponent.Play(false, animation));
        }
    }
}
