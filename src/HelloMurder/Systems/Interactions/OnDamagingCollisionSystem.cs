using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Messages;
using Murder.Diagnostics;
using Murder.Services;
using System.Numerics;

namespace HelloMurder.Systems.Interactions
{
    [Filter(typeof(HealthComponent))]
    [Messager(typeof(DamagingCollisionMessage))]
    internal class OnDamagingCollisionSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            GameLogger.Log("Processing Collision on specific entity");
            var msg = (DamagingCollisionMessage)message;
            var hpAfterDamage = entity.GetHealth().Damage(msg.DamageDealt);
            entity.SetHealth(hpAfterDamage);

            // Play any collision vfx, sfx

            // Cleanup dead entities
            if (hpAfterDamage.Health <= 0)
            {
                // optional way to build
                //entity.SendMessage(new FatalDamageMessage());

                CoroutineServices.RunCoroutine(entity, KillAndCleanUp(entity));
            }
        }

        private IEnumerator<Wait> KillAndCleanUp(Entity entity)
        {
            entity.SetCollider(entity.GetCollider().SetLayer(0));
            entity.SetAgentImpulse(Vector2.Zero);
            entity.SetFriction(1.0f);
            
            // In case the entity has these
            entity.RemovePlayer();
            entity.RemoveMoveToPlayer();

            while (entity.TryGetVelocity() != null)
            {
                yield return Wait.NextFrame;
            }

            entity.Destroy();
        }
    }
}
