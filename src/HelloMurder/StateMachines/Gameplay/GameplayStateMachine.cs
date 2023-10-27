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
using Murder.Components;
using Murder.Core.Particles;
using Murder.Assets;
using Murder.Core.Graphics;
using HelloMurder.Core;
using Murder.Services;

namespace HelloMurder.StateMachines.Gameplay
{
    internal class GameplayStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(EnemySpawnerDataAsset))]
        private readonly Guid _enemySpawnDataId = Guid.Empty;

        [JsonProperty, GameAssetId<PrefabAsset>]
        private readonly Guid _radioPrefab = Guid.Empty;

        [JsonProperty]
        private readonly float _bombWindRadius = 64f;

        [JsonProperty]
        private readonly string _windWarningText = "";

        private readonly LibraryAsset _libraryAsset;

        private Entity? _player;
        private Entity? _radio;

        private bool _showWindWarning;
        private bool _showControls;
        private bool _reachedEnd;
        private float _lastStartedTime = 0;

        public GameplayStateMachine() {

            _libraryAsset = LibraryServices.GetLibrary();
            State(GameStart);
        }

        private IEnumerator<Wait> GameStart()
        {
            World.DeactivateSystem<PlayerInputSystem>();
            Entity.SetGameplayStateMachine();
            Entity.SetCustomDraw(DrawMessage);

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
            _radio = AssetServices.Create(World, _radioPrefab);
            var bottomCenter = _libraryAsset.Bounds.Center;
            bottomCenter.Y += _libraryAsset.Bounds.Height / 2f;
            _radio.SetGlobalPosition(bottomCenter);
            var bottomMinusRadioHeight = bottomCenter;
            bottomMinusRadioHeight.Y -= 20f;

            while (_player != null && Vector2.Distance(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center) > 5f)
            {
                var pos = Vector2.Lerp(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center, Game.DeltaTime);
                _player.SetGlobalPosition(pos);
                _player.SetPlayerSpeed(_player.GetPlayerSpeed().Approach(3f, 1f * Game.DeltaTime));

                var radioPos = Vector2.Lerp(_radio.GetGlobalTransform().Vector2, bottomMinusRadioHeight, Game.DeltaTime);
                _radio.SetGlobalPosition(radioPos);
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

            _lastStartedTime = Game.NowUnscaled;
            _showWindWarning = true;
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
            _showWindWarning = false;
            yield return Wait.NextFrame;
            
            var bottomCenter = _libraryAsset.Bounds.Center;
            bottomCenter.Y += (_libraryAsset.Bounds.Height / 2f) + 30f;

            while (_radio != null && Vector2.Distance(_radio.GetGlobalTransform().Vector2, bottomCenter) > 5f)
            {
                var radioPos = Vector2.Lerp(_radio.GetGlobalTransform().Vector2, bottomCenter, Game.DeltaTime);
                _radio.SetGlobalPosition(radioPos);
                yield return Wait.ForFrames(1);
            }

            BeginEnemySpawn();
            World.ActivateSystem<PlayerInputSystem>();
        }

        private void BeginEnemySpawn()
        {
            var enemySpawnManager = new StateMachineComponent<EnemySpawnManager>(new EnemySpawnManager(_enemySpawnDataId));
            World.AddEntity(enemySpawnManager);
            var flakSpawnManager = new StateMachineComponent<FlakSpawnManager>(new FlakSpawnManager());
            World.AddEntity(flakSpawnManager);
        }

        private IEnumerator<Wait> CoreLoop()
        {
            yield return Wait.ForMessage<GameStepCompleteMessage>();
        }

        private IEnumerator<Wait> EndSequenceStart()
        {
            yield return Wait.NextFrame;
        }

        private void DrawMessage(RenderContext render)
        {
            if (_showWindWarning)
            {
                float timeSinceAppeared = Game.NowUnscaled - _lastStartedTime;
                int currentLength = Calculator.CeilToInt(Calculator.ClampTime(timeSinceAppeared, 2f /* dialog duration */) * _windWarningText.Length);

                if (_reachedEnd || currentLength == _windWarningText.Length)
                {
                    _reachedEnd = true;

                    currentLength = _windWarningText.Length;
                }
                int maxWidth = 230;

                float dialogWidth = Math.Min(
                    Game.Data.GetFont(100).GetLineWidth(_windWarningText.AsSpan().Slice(0, currentLength)), maxWidth);

                var bottomCenter = _libraryAsset.Bounds.Center;
                bottomCenter.X -= dialogWidth / 2f;
                bottomCenter.Y += _libraryAsset.Bounds.Height / 2f - 64f;
                Vector2 textPosition = bottomCenter;

                Game.Data.GetFont(100).Draw(
                    render.UiBatch,
                    _windWarningText,
                    position: textPosition,
                    alignment: new Vector2(0, 0),
                    scale: new Vector2(1, 1),
                    sort: .5f,
                    color: Palette.Colors[9],
                    strokeColor: null,
                    shadowColor: Palette.Colors[0],
                    maxWidth: maxWidth,
                    visibleCharacters: currentLength
                );

                /*
                if (_playNextSound < Game.NowUnscaled && currentLength < line.Text.Length)
                {
                    if (isRightSpeaker)
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(_speaker == SpeakerKind.Granny ?
                            LibraryServices.GetRoadLibrary().GrandmaTextBeep :
                            LibraryServices.GetRoadLibrary().CarTextBeep, isLoop: false);
                    }
                    else
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().DaggersTextBeep, isLoop: false);
                    }

                    _playNextSound = Game.NowUnscaled + 0.09f;
                }
                */
            }
            if (_showControls)
            {

            }
        }
    }
}