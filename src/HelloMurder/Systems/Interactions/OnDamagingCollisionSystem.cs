using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Messages;
using HelloMurder.Services;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Messages;
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
            var hpComponent = entity.GetHealth();
            var hpBeforeDamage = hpComponent.Health;
            var hpAfterDamage = hpComponent.Damage(msg.DamageDealt);
            entity.SetHealth(hpAfterDamage);

            // Play any collision vfx, sfx
            if (entity.HasEnemy())
            {
                LibraryServices.Explode(0, world, msg.Center);

                var mw = (MonoWorld)world;
                mw.Camera.Shake(2f, .2f);
            }

            // Message damaged entities, don't message already dead ones    
            if (hpBeforeDamage <= 0)
            {
                return;
            }
            else if (hpAfterDamage.Health <= 0)
            {
                // Send fatal damage message
                entity.SendMessage(new FatalDamageMessage());
            }
            else
            {
                // Send damaged message
                entity.SendMessage(new HealthDamagedMessage(msg.DamageDealt));
            }
        }

        private IEnumerator<Wait> KillAndCleanUp(Entity entity)
        {
            entity.SetCollider(entity.GetCollider().SetLayer(0));
            entity.SetAgentImpulse(Vector2.Zero);
            entity.SetVelocity(Vector2.Zero);
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
