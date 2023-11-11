using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using BombsAway.Components;
using BombsAway.Core;
using BombsAway.Core.Sounds;
using BombsAway.Messages;
using BombsAway.Services;
using Murder;
using Murder.Components;
using Murder.Utilities;

namespace BombsAway.Systems.Player
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

            var wind = entity.TryGetWind();

            if (wind == null) return;

            // Spawn a bomb
            Guid prefab = LibraryServices.GetLibrary().BombPrefab;

            Entity e = AssetServices.Create(world, prefab);
            e.SetTransform(entity.GetGlobalTransform());

            var playerPos = entity.GetGlobalTransform().Vector2;
            var bombOffset = playerPos + wind.Value.WindVector;
            var mover = new MoveToComponent(bombOffset);
            e.SetMoveTo(mover);
            
            SpriteComponent sprite = e.GetSprite();
            sprite = sprite.Play(false, "falling", "damage", "miss");
            
            e.SetSprite(sprite);

            _timeSinceLastBomb = Game.Now;

            BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().BombDrop, Murder.Core.Sounds.SoundProperties.None);
        }
    }
}
