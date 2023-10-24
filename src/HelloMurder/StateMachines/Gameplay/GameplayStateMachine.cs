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
using Murder.Core.Particles;

namespace HelloMurder.StateMachines.Gameplay
{
    internal class GameplayStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(EnemySpawnerDataAsset))]
        private readonly Guid _enemySpawnDataId = Guid.Empty;

        [JsonProperty]
        private readonly float _bombWindRadius = 64f;

        private readonly LibraryAsset _libraryAsset;

        private Entity? _player;

        public GameplayStateMachine() {

            _libraryAsset = LibraryServices.GetLibrary();
            State(GameStart);
        }

        private IEnumerator<Wait> GameStart()
        {
            World.DeactivateSystem<PlayerInputSystem>();
            Entity.SetGameplayStateMachine();

            _player = World.GetUniqueEntity<PlayerComponent>();
            var landIcon = _player.TryFetchChild("wind_indicator_icon");
            landIcon?.Deactivate();

            yield return Wait.ForRoutine(FlyPlayerOnScreen());
            yield return Wait.ForRoutine(ShowWindIndicator());
            yield return Wait.ForRoutine(AnimateWindIndicator());
            yield return Wait.ForRoutine(HideWindIndicator());
            yield return Wait.ForRoutine(CoreLoop());
            yield return Wait.ForRoutine(EndSequenceStart());
        }

        private IEnumerator<Wait> FlyPlayerOnScreen()
        {
            while (_player != null && Vector2.Distance(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center) > 5f)
            {
                var pos = Vector2.Lerp(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center, Game.DeltaTime);
                _player.SetGlobalPosition(pos);
                _player.SetPlayerSpeed(_player.GetPlayerSpeed().Approach(3f, 1f * Game.DeltaTime));
                yield return Wait.ForFrames(1);
            }

            SetWind();
        }

        private void SetWind()
        {
            float angle = 2.0f * MathF.PI * Game.Random.NextFloat();

            Vector2 windDir =
                new Vector2(
                    _bombWindRadius * MathF.Cos(angle),
                    _bombWindRadius * MathF.Sin(angle));
            WindComponent wind = new WindComponent(windDir);
            _player?.SetWind(wind);
        }

        private IEnumerator<Wait> ShowWindIndicator()
        {
            // Tell the Wind UI To play
            if (_player == null)
                yield break;

            var wind = _player.GetWind();
            var windParticle = _player.TryFetchChild("wind_indicator");
            windParticle?.Activate();

            var bombEndpoint = _player.GetGlobalTransform().Vector2 + (wind.WindVector * 0.75f);

            while (windParticle != null && Vector2.Distance(windParticle.GetGlobalTransform().Vector2, bombEndpoint) > 2f)
            {
                var pos = windParticle.GetGlobalTransform().Vector2 + wind.WindVector.Normalized() * 20f * Game.DeltaTime;
                windParticle.SetGlobalPosition(pos);
                yield return Wait.ForFrames(1);
            }


            if (windParticle != null)
            {
                WorldParticleSystemTracker worldTracker = World.GetUnique<ParticleSystemWorldTrackerComponent>().Tracker;

                worldTracker.Deactivate(windParticle.EntityId);
            }

            yield return Wait.ForSeconds(1f);
            windParticle?.Deactivate();
            var landIcon = _player.TryFetchChild("wind_indicator_icon");
            landIcon?.SetGlobalPosition(bombEndpoint);
            landIcon?.Activate();
            landIcon?.GetSprite().Play(false);
        }

        private IEnumerator<Wait> AnimateWindIndicator()
        {
            var landIcon = _player?.TryFetchChild("wind_indicator_icon");
            yield return Wait.ForSeconds(4f);
            landIcon?.Deactivate();
        }

        private IEnumerator<Wait> HideWindIndicator()
        {
            var windParticle = _player?.TryFetchChild("wind_indicator");
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