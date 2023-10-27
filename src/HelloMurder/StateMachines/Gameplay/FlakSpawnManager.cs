using Bang.StateMachines;
using HelloMurder.Services;
using Murder;
using Murder.Core.Geometry;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Numerics;
using Bang.Entities;
using HelloMurder.Assets;
using Murder.Components;
using HelloMurder.Components;

namespace HelloMurder.StateMachines.Gameplay
{
    public class FlakSpawnManager : StateMachine
    {
        [JsonProperty]
        private readonly float _timeBetweenFlakSeconds = 1.0f;

        public FlakSpawnManager()
        {
            State(Level);
        }

        private IEnumerator<Wait> Level()
        {
            Entity.SetFlakSpawnManager();
            LibraryAsset library = LibraryServices.GetLibrary();
            var flakPrefab = library.FlakPrefab;
            var bounds = library.Bounds;
            while (true)
            {
                SpawnEntity(flakPrefab, bounds);

                yield return Wait.ForSeconds(_timeBetweenFlakSeconds);
            }
        }

        private void SpawnEntity(Guid prefabToSpawn, IntRectangle bounds)
        {
            var prefab = Game.Data.GetPrefab(prefabToSpawn);
            var entity = prefab.CreateAndFetch(World);

            Vector2 position;
            var player = World.TryGetUniqueEntity<PlayerComponent>();
            if (player != null && Game.Random.TryWithChanceOf(20))
            {
                position = player.GetGlobalTransform().Vector2;
            }
            else
            {
                position =
                    new Vector2(
                    Game.Random.Next(0, bounds.Width),
                    Game.Random.Next(0, bounds.Height));
            }
            entity.SetGlobalPosition(position);

            SpriteComponent sprite = entity.GetSprite();
            sprite = sprite.Play(false, "rising", "damage", "smoke_fading");
            entity.SetSprite(sprite);
        }
    }
}
