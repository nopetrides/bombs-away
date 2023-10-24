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
using Murder.Components;

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

            yield return Wait.ForRoutine(FlyPlayerOnScreen());
            yield return Wait.ForRoutine(ShowWindIndicator());
            yield return Wait.ForRoutine(AnimateWindIndicator());
            yield return Wait.ForRoutine(HideWindIndicator());
            yield return Wait.ForRoutine(CoreLoop());
            yield return Wait.ForRoutine(EndSequenceStart());
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

            SetWind();
        }

        private void SetWind()
        {
            var player = World.GetUniqueEntity<PlayerComponent>();

            float angle = 2.0f * MathF.PI * Game.Random.NextFloat();

            Vector2 windDir =
                new Vector2(
                    _bombWindRadius * MathF.Cos(angle),
                    _bombWindRadius * MathF.Sin(angle));
            WindComponent wind = new WindComponent(windDir);
            player.SetWind(wind);
        }

        private IEnumerator<Wait> ShowWindIndicator()
        {
            // Tell the Wind UI To play
            var player = World.GetUniqueEntity<PlayerComponent>();
            var wind = player.GetWind();
            var windParticle = player.TryFetchChild("wind_indicator");
            windParticle?.Activate();

            var bombEndpoint = player.GetGlobalTransform().Vector2 + (wind.WindVector * 0.75f);

            while (windParticle != null && Vector2.Distance(windParticle.GetGlobalTransform().Vector2, bombEndpoint) > 1f)
            {
                var pos = windParticle.GetGlobalTransform().Vector2 + wind.WindVector.Normalized() * 20f * Game.DeltaTime;
                windParticle.SetGlobalPosition(pos);
                yield return Wait.ForFrames(1);
            }
        }

        private IEnumerator<Wait> AnimateWindIndicator()
        {
            yield return Wait.ForSeconds(0.2f);
            yield return Wait.ForSeconds(0.2f);
            yield return Wait.ForSeconds(0.2f);
            yield return Wait.ForSeconds(0.2f);
        }

        private IEnumerator<Wait> HideWindIndicator()
        {
            var player = World.GetUniqueEntity<PlayerComponent>();
            var windParticle = player.TryFetchChild("wind_indicator");
            windParticle?.Deactivate();
            yield return Wait.NextFrame;
            BeginEnemySpawn();
            World.ActivateSystem<PlayerInputSystem>();
        }

        private void BeginEnemySpawn()
        {
            var enemySpawnManager = new StateMachineComponent<EnemyLevelManager>(new EnemyLevelManager(_enemySpawnDataId));
            World.AddEntity(enemySpawnManager);
        }

        private IEnumerator<Wait> CoreLoop()
        {
            yield return Wait.ForMessage<GameStepCompleteMessage>();
        }

        private IEnumerator<Wait> EndSequenceStart()
        {
            yield return Wait.NextFrame;
        }
    }
}