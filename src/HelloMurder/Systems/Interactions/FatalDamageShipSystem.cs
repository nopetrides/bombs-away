

using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
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
            LibraryServices.Explode(0, world, entity.GetGlobalTransform().Vector2);

            Murder.Components.SpriteComponent spriteComponent = entity.GetSprite();
            
            entity.SetSprite(spriteComponent.PlayOnce("hit_sunk", false));

            entity.SetDestroyOnAnimationComplete(false);
        }
    }
}
