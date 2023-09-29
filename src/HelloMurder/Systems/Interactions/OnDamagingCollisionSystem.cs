using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Messages;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder.Systems.Interactions
{
    /// <summary>
    ///     Handle damaging collision interactions on a specific entity
    ///     This can run twice per collision, once per entity
    /// </summary>
    [Filter(typeof(HealthComponent))]
    [Messager(typeof(DamagingCollisionMessage))]
    internal class OnDamagingCollisionSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            GameLogger.Log("On Collision Processed");
            var msg = (DamagingCollisionMessage)message;
            var hpAfterDamage = entity.GetHealth().Damage(msg.DamageDealt);

            // Play any collision vfx, sfx

            // Cleanup dead entities
            if (hpAfterDamage.CurrentHealth <= 0)
            {
                entity.SendMessage(new FatalDamageMessage()); // no listeners
                CoroutineServices.RunCoroutine(entity, KillAndCleanUp(entity));
            }
        }

        /// <summary>
        /// Remove components that shouldn't stay active,
        /// Play any death effects,
        /// Wait for any death animation stuff to finish,
        /// Finally destroy the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerator<Wait> KillAndCleanUp(Entity entity)
        {
            entity.RemoveCollider();
            entity.SetAgentSpeedOverride(0, 0);
            // Magnitude, may need to be non-zero check due to floating points
            while (entity.TryGetVelocity() != null)
            {
                yield return Wait.NextFrame;
            }

            // Cleanup Entity
            entity.Destroy();
        }
    }
}
