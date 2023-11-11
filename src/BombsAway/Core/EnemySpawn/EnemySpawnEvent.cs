using Murder.Assets;
using Murder.Attributes;

namespace BombsAway.Core.EnemySpawn
{
    public readonly struct EnemySpawnEvent
    {
        // How fast this enemy moves
        public readonly float EnemyImpulse = 50f;
        // Will the enemy always move towards the player, or just in a straight line?
        public readonly bool ChasePlayer = false;

        [GameAssetId<PrefabAsset>]
        public readonly Guid PrefabToSpawn = Guid.Empty;

        // How long into the level before this type of enemy can spawn
        public readonly float SpawnTimeStart = 0f;

        public EnemySpawnEvent()
        {
        }
    }
}
