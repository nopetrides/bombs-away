using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Messages;
using Murder.Diagnostics;
using Murder.Messages;

namespace HelloMurder.Systems.Physics
{
    /// <summary>
    /// System that is used to handle collisions between two entities during the physics step
    /// </summary>
    [Filter(typeof(HealthComponent))]
    [Messager(typeof(CollidedWithMessage))]
    public class CollidingDamageSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            GameLogger.Log("Handling Collision");
            var msg = (CollidedWithMessage)message;
            var other = world.GetEntity(msg.EntityId);

            HandleCollision(entity, other);
        }

        public static void HandleCollision(Entity entity1, Entity entity2)
        {
            entity1.SendMessage(new DamagingCollisionMessage(entity2.EntityId, entity2.TryGetDealsDamage()?.Damage ?? 0));
            entity2.SendMessage(new DamagingCollisionMessage(entity1.EntityId, entity1.TryGetDealsDamage()?.Damage ?? 0));
        }
    }
}
