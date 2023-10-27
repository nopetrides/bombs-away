using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using HelloMurder.Assets;
using HelloMurder.Components;
using HelloMurder.Services;
using Murder;
using Murder.Core;
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
            LibraryServices.Explode(1, world, entity.GetGlobalTransform().Vector2 + new Vector2(0, 16));
            var monoWorld = (MonoWorld)world;
            monoWorld.Camera.Shake(0, 0);

            CoroutineServices.RunCoroutine(world, KillAndCleanup(world, entity));
        }

        private IEnumerator<Wait> KillAndCleanup(World world, Entity entity)
        {
            //TODO player destroyed animation animation
            //SpriteComponent spriteComponent = entity.GetSprite();
            //entity.SetSprite(spriteComponent.PlayOnce("death", false));

            entity.SetDestroyAtTime(Game.Now + 2f); // TODO replace
            yield return Wait.ForSeconds(2.5f);

            Game.Instance.QueueWorldTransition(LibraryServices.GetLibrary().GameOver);
        }
    }
}
