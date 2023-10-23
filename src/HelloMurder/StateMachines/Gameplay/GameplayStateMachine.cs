using Bang.StateMachines;
using HelloMurder.Assets;
using HelloMurder.Components;
using HelloMurder.Messages;
using HelloMurder.Services;
using Murder.Utilities;
using Bang.Entities;
using Murder;
using System.Numerics;
using HelloMurder.Systems;
using Murder.Attributes;
using Newtonsoft.Json;
using Murder.Core.Geometry;
using Murder.Assets;

namespace HelloMurder.StateMachines.Gameplay
{
    internal class GameplayStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(EnemySpawnerDataAsset))]
        private readonly Guid _enemySpawnDataId = Guid.Empty;

        [JsonProperty]
        private readonly float _bombWindRadius = 64f;

        private readonly LibraryAsset _libraryAsset;

        public GameplayStateMachine() {

            _libraryAsset = LibraryServices.GetLibrary();
            State(GameStart);
        }

        private IEnumerator<Wait> GameStart()
        {
            World.DeactivateSystem<PlayerInputSystem>();
            Entity.SetGameplayStateMachine();
            yield return Wait.NextFrame;
            while (true)
            {
                yield return Wait.ForRoutine(FlyPlayerOnScreen());
            }
        }

        private IEnumerator<Wait> FlyPlayerOnScreen()
        {
            var player = World.GetUniqueEntity<PlayerComponent>();
            while (Vector2.Distance(player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center) > 5f)
            {
                var pos = Vector2.Lerp(player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center, Game.DeltaTime);
                player.SetGlobalPosition(pos);
                player.SetPlayerSpeed(player.GetPlayerSpeed().Approach(3f, 1f * Game.DeltaTime));
                yield return Wait.ForFrames(1);
            }

            World.ActivateSystem<PlayerInputSystem>();
            yield return Wait.ForRoutine(ShowWindIndicator());
        }

        private IEnumerator<Wait> ShowWindIndicator()
        {
            var player = World.GetUniqueEntity<PlayerComponent>();

            float angle = 2.0f * MathF.PI * Game.Random.NextFloat();

            Vector2 windDir = 
                new Vector2(
                    _bombWindRadius * MathF.Cos(angle), 
                    _bombWindRadius * MathF.Sin(angle));
            WindComponent wind = new WindComponent(windDir);
            player.SetWind(wind);

            //yield return Wait.ForMessage<GameStepCompleteMessage>();


            yield return Wait.ForRoutine(HideWindIndicator());
        }

        private IEnumerator<Wait> HideWindIndicator()
        {
            yield return Wait.NextFrame;
            yield return Wait.ForRoutine(BeginEnemySpawn());
        }

        private IEnumerator<Wait> BeginEnemySpawn()
        {
            var enemySpawnManager = new StateMachineComponent<EnemyLevelManager>(new EnemyLevelManager(_enemySpawnDataId));
            World.AddEntity(enemySpawnManager);
            yield return Wait.ForRoutine(CoreLoop());
        }

        private IEnumerator<Wait> CoreLoop()
        {
            yield return Wait.ForMessage<GameStepCompleteMessage>();
            GoTo(EndSequenceStart);
        }

        private IEnumerator<Wait> EndSequenceStart()
        {
            yield return Wait.NextFrame;
        }
    }
}