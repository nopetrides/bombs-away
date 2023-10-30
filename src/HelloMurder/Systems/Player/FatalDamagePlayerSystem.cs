using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Services;
using Murder;
using Murder.Components;
using Murder.Core;
using Murder.Core.Particles;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace HelloMurder.Systems.Player
{
    [Filter(typeof(PlayerComponent))]
    [Messager(typeof(FatalDamageMessage))]
    internal class FatalDamagePlayerSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            world.DeactivateSystem<PlayerInputSystem>();
            var monoWorld = (MonoWorld)world;
            monoWorld.Camera.Shake(1f, 2.5f);

            CoroutineServices.RunCoroutine(world, KillAndCleanup(world, entity));
        }

        private IEnumerator<Wait> KillAndCleanup(World world, Entity entity)
        {
            AgentSpriteComponent spriteComponent = entity.GetAgentSprite();
            var spriteGuid = LibraryServices.GetLibrary().PlayerDeath;
            var deathPosition = entity.GetGlobalTransform().Vector2;

            entity.RemoveAgentSprite();
            var animation = System.Collections.Immutable.ImmutableArray<string>.Empty;
            animation = animation.Add("player_death");
            entity.AddOrReplaceComponent(new SpriteComponent(spriteGuid, Vector2.Zero,animation,0,false,false,Murder.Core.Graphics.OutlineStyle.None, 0, 0));

            entity.SetDestroyOnAnimationComplete();
            foreach (var c in entity.Children)
            {
                var child = world.GetEntity(c);
                if (!child.HasParticleSystem())
                    continue;
                WorldParticleSystemTracker worldTracker = world.GetUnique<ParticleSystemWorldTrackerComponent>().Tracker;
                worldTracker.Deactivate(c);
                child.Unparent();
            }

            yield return Wait.ForMessage<AnimationCompleteMessage>(entity);

            LibraryServices.Explode(1, world, deathPosition + new Vector2(0, 16));

            var monoWorld = (MonoWorld)world; 
            monoWorld.Camera.Shake(3f, 1f);

            yield return Wait.ForSeconds(2f);

            Game.Instance.QueueWorldTransition(LibraryServices.GetLibrary().GameOver);
        }
    }
}
