using Bang.StateMachines;
using System.Collections.Immutable;
using Bang.Entities;
using Murder.Diagnostics;
using HelloMurder.Assets;
using Murder;
using Murder.Utilities;
using Murder.Core;
using System.Numerics;
using HelloMurder.Core.EnemySpawn;
using Murder.Attributes;
using Newtonsoft.Json;

namespace HelloMurder.StateMachines.Gameplay
{
    public class EnemyLevelManager : StateMachine
    {
        // Displayed in the instance editor
        [JsonProperty, GameAssetId(typeof(EnemySpawnerDataAsset))]
        private readonly Guid _enemySpawnDataId = Guid.Empty;

        private int _currentPossibleEnemy = 0;

        private ImmutableArray<EnemySpawnEvent> _possibleEnemiesShuffled = ImmutableArray<EnemySpawnEvent>.Empty;
        private ImmutableArray<EnemySpawnEvent> _possibleEnemies = ImmutableArray<EnemySpawnEvent>.Empty;

        /// <summary>
        /// For constructing this from something like a level manager that knows that enemies will spawn in the level
        /// </summary>
        /// <param name="enemySpawnData"></param>
        public EnemyLevelManager(Guid enemySpawnData)
        {
            _enemySpawnDataId = enemySpawnData;
            State(Level);
        }

        /// <summary>
        /// Used when running the state machine directly in the world. Uses the JsonProperty of _enemySpawnDataId
        /// </summary>
        public EnemyLevelManager()
        {
            State(Level);
        }

        private IEnumerator<Wait> Level()
        {
            Entity.SetEnemyLevelManager();
            EnemySpawnerDataAsset spawnerData = Game.Data.GetAsset<EnemySpawnerDataAsset>(_enemySpawnDataId);

            if (spawnerData.EnemySpawns.Length == 0)
            {
                GameLogger.Warning("There are no enemies here! Was this intentional?");
                yield break;
            }

            float time = 0;
            float currentSpawnDelay = spawnerData.StartingSpawnDelay;
            while (true)
            {
                // check the spawns list, see if there is a valid spawn to add to the current spawn list
                if (_possibleEnemies.Length < spawnerData.EnemySpawns.Length)
                {
                    for (int i = _possibleEnemies.Length; i < spawnerData.EnemySpawns.Length; i++)
                    {
                        if (time > spawnerData.EnemySpawns[i].SpawnTimeStart)
                        {
                            _possibleEnemies = _possibleEnemies.Add(spawnerData.EnemySpawns[i]);
                            _possibleEnemiesShuffled = Shuffle(_possibleEnemies);
                            _currentPossibleEnemy = 0;
                        }
                    }
                }

                if (_currentPossibleEnemy >= _possibleEnemiesShuffled.Length)
                {
                    _currentPossibleEnemy = 0;
                    _possibleEnemiesShuffled = Shuffle(_possibleEnemies);
                }

                // Get the spawn data about this enemy type
                EnemySpawnEvent currentSpawn = spawnerData.EnemySpawns[_currentPossibleEnemy];
                // Do the spawn!
                SpawnEntity(currentSpawn.PrefabToSpawn, currentSpawn.EnemyImpulse, currentSpawn.ChasePlayer);

                // increment the enemy to spawn next
                if (_possibleEnemiesShuffled.Length > 1) 
                    _currentPossibleEnemy++;
                // Count time passed
                time += currentSpawnDelay;
                // Wait to spawn the enext enemy
                yield return Wait.ForSeconds(currentSpawnDelay);
            }
        }

        private void SpawnEntity(Guid prefabToSpawn, float enemyImpulse, bool chasePlayer)
        {
            var prefab = Game.Data.GetPrefab(prefabToSpawn);
            var entity = prefab.CreateAndFetch(World);

            var world = (MonoWorld)World;

            Vector2 position = Vector2.Zero;
            var bounds = world.Camera.Bounds;

            position = bounds.TopLeft;
            position.X += Game.Random.NextFloat(0, bounds.Width);

            entity.SetGlobalPosition(position);

            entity.SetAgent(enemyImpulse, enemyImpulse, 0f);
            if (chasePlayer) 
                entity.SetMoveToPlayer();
        }

        public ImmutableArray<EnemySpawnEvent> Shuffle(ImmutableArray<EnemySpawnEvent> array)
        {
            // Copy the elements to a mutable list
            List<EnemySpawnEvent> list = new List<EnemySpawnEvent>(array);

            // Shuffle the list using Fisher-Yates algorithm
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Game.Random.Next(i + 1);
                EnemySpawnEvent temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            // Create a new ImmutableArray<Guid> from the shuffled list
            return ImmutableArray.CreateRange(list);
        }
    }
}