using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Core;
using HelloMurder.Core.Sounds;
using HelloMurder.Messages;
using HelloMurder.Services;
using Murder;
using Murder.Components;
using Murder.Utilities;

namespace HelloMurder.Systems.Player
{
    [Filter(typeof(PlayerComponent))]
    [Messager(typeof(AgentInputMessage))]
    public class SpawnBombSystem : IMessagerSystem
    {
        private float _timeSinceLastBomb = -1f;
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (message == null) return;

            var input = (AgentInputMessage)message;
            if (input.Button != InputButtons.Attack) return;

            // Reload time
            if (Game.Now < _timeSinceLastBomb + 1f) return;

            // Spawn a bomb
            Guid prefab = LibraryServices.GetLibrary().BombPrefab;

            Entity e = AssetServices.Create(world, prefab);
            e.SetTransform(entity.GetGlobalTransform());

            var playerPos = entity.GetGlobalTransform().Vector2;
            var bombOffset = playerPos + entity.GetWind().WindVector;
            var mover = new MoveToComponent(bombOffset);
            e.SetMoveTo(mover);
            
            SpriteComponent sprite = e.GetSprite();
            sprite = sprite.Play(false, "falling", "damage", "miss");
            
            e.SetSprite(sprite);

            _timeSinceLastBomb = Game.Now;

            HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().BombDrop, Murder.Core.Sounds.SoundProperties.None);
        }
    }
}
