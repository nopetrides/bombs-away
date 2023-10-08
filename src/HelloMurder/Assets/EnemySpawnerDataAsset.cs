using HelloMurder.Core.EnemySpawn;
using Murder.Assets;
using System.Collections.Immutable;
using System.Numerics;

namespace HelloMurder.Assets
{
    public class EnemySpawnerDataAsset : GameAsset
    {
        public override char Icon => '';
        public override string EditorFolder => "#EnemyTypes";
        public override Vector4 EditorColor => new Vector4(.8f, 1f, .35f, 1f);

        // Time between spawns
        public float StartingSpawnDelay = 1f;

        public ImmutableArray<EnemySpawnEvent> EnemySpawns = ImmutableArray<EnemySpawnEvent>.Empty;
    }
}
