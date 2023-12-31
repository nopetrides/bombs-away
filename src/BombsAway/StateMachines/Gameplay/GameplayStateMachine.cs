﻿using Bang.StateMachines;
using BombsAway.Assets;
using BombsAway.Components;
using BombsAway.Messages;
using BombsAway.Services;
using Murder.Utilities;
using Bang.Entities;
using Murder;
using System.Numerics;
using BombsAway.Systems;
using Murder.Attributes;
using Newtonsoft.Json;
using Murder.Components;
using Murder.Core.Particles;
using Murder.Assets;
using Murder.Core.Graphics;
using BombsAway.Core;
using Murder.Services;
using BombsAway.Core.Sounds;
using BombsAway.Data;

namespace BombsAway.StateMachines.Gameplay
{
    /// <summary>
    /// Handles the gameplay intro sequence
    /// </summary>
    internal class GameplayStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(EnemySpawnerDataAsset))]
        private readonly Guid _enemySpawnDataId = Guid.Empty;

        [JsonProperty, GameAssetId<PrefabAsset>]
        private readonly Guid _radioPrefab = Guid.Empty;

        [JsonProperty, GameAssetId<PrefabAsset>]
        private readonly Guid _controlsPrefab = Guid.Empty;

        [JsonProperty]
        private readonly float _bombWindRadius = 64f;

        [JsonProperty]
        private readonly string _windWarningText = "";

        [JsonProperty]
        private readonly string _controlsText = "";

        private readonly LibraryAsset _libraryAsset;

        private Entity? _player;
        private Entity? _radio;
        private Entity? _controls;

        private bool _showWindWarning;
        private bool _showingControls;
        private bool _reachedEnd;
        private float _lastStartedTime = 0;
        private bool _anyInput;

        private float _playNextSound = 0;

        private bool _cantSkip;

        public GameplayStateMachine() {

            _libraryAsset = LibraryServices.GetLibrary();
            State(GameStart);
        }

        private IEnumerator<Wait> GameStart()
        {
            //World.DeactivateSystem<PlayerInputSystem>();
            Entity.SetGameplayStateMachine();
            Entity.SetCustomDraw(DrawMessage);
            BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().GameMusic, Murder.Core.Sounds.SoundProperties.Persist | Murder.Core.Sounds.SoundProperties.StopOtherMusic);

            BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().Turboprop, Murder.Core.Sounds.SoundProperties.Persist);
            BombsAwaySoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Roll, 0f);
            BombsAwaySoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Thrust, 0f);

            _player = World.GetUniqueEntity<PlayerComponent>();
            var landIcon = _player.TryFetchChild("wind_indicator_icon");
            landIcon?.Deactivate();

            var flakWarning = _player.TryFetchChild("flak_warning");
            flakWarning?.Deactivate();

            BombsAwaySaveData save = SaveServices.GetOrCreateSave();

            // Workaround, since saves are not working between sessions
            BombsAwayPreferences pref = (BombsAwayPreferences)Game.Preferences;

            _cantSkip = pref.HighScore == 0;


            yield return Wait.ForFrames(1);
            yield return Wait.ForRoutine(FlyPlayerOnScreen());
            yield return Wait.ForRoutine(ShowWindIndicator());
            yield return Wait.ForRoutine(AnimateWindIndicator());
            yield return Wait.ForRoutine(HideWindIndicator());
            yield return Wait.ForRoutine(ShowControls());
            yield return Wait.ForRoutine(WaitForInput());
            yield return Wait.ForRoutine(HideControls());
        }

        private IEnumerator<Wait> FlyPlayerOnScreen()
        {
            if (_cantSkip) World.DeactivateSystem<PlayerInputSystem>();

            _radio = AssetServices.Create(World, _radioPrefab);
            var bottomCenter = _libraryAsset.Bounds.Center;
            bottomCenter.Y += _libraryAsset.Bounds.Height / 2f;
            _radio.SetGlobalPosition(bottomCenter);
            var bottomMinusRadioHeight = bottomCenter;
            bottomMinusRadioHeight.Y -= 20f;

            _lastStartedTime = Game.Now;
            _showWindWarning = true;

            while (_player != null && (_cantSkip || !_player.HasVelocity()) && Vector2.Distance(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center) > 5f)
            {
                var pos = Vector2.Lerp(_player.GetGlobalTransform().Vector2, _libraryAsset.Bounds.Center, Game.DeltaTime);
                _player.SetGlobalPosition(pos);
                _player.SetPlayerSpeed(_player.GetPlayerSpeed().Approach(3f, 1f * Game.DeltaTime));

                var radioPos = Vector2.Lerp(_radio.GetGlobalTransform().Vector2, bottomMinusRadioHeight, Game.DeltaTime);
                _radio.SetGlobalPosition(radioPos);
                yield return Wait.ForFrames(1);
            }

            _player.SetPlayerSpeed(3f);
            _radio.SetGlobalPosition(bottomMinusRadioHeight);

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

            //_lastStartedTime = Game.NowUnscaled;
            //_showWindWarning = true;
            var wind = _player.GetWind();
            var windParticle = _player.TryFetchChild("wind_indicator");
            windParticle?.Activate();

            var bombEndpoint = (wind.WindVector * 0.75f);

            while (windParticle != null && Vector2.Distance(windParticle.GetPosition().ToVector2(), bombEndpoint) > 2f)
            {
                var pos = Vector2.Lerp(windParticle.GetPosition().ToVector2(), bombEndpoint, Game.DeltaTime);
                windParticle.SetLocalPosition(pos);
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
            landIcon?.SetLocalPosition(bombEndpoint);
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

            while (_radio != null && (_cantSkip || !_player.HasVelocity()) && Vector2.Distance(_radio.GetGlobalTransform().Vector2, bottomCenter) > 5f)
            {
                var radioPos = Vector2.Lerp(_radio.GetGlobalTransform().Vector2, bottomCenter, Game.DeltaTime);
                _radio.SetGlobalPosition(radioPos);
                yield return Wait.ForFrames(1);
            }

            _radio?.Destroy();
        }

        private IEnumerator<Wait> ShowControls()
        {
            if (_cantSkip) World.ActivateSystem<PlayerInputSystem>();

            _controls = AssetServices.Create(World, _controlsPrefab);
            var bottomCenter = _libraryAsset.Bounds.Center;
            bottomCenter.Y += _libraryAsset.Bounds.Height / 2f;
            _controls.SetGlobalPosition(bottomCenter);
            var bottomMinusControlsHeight = bottomCenter;
            bottomMinusControlsHeight.Y -= 30f;

            _reachedEnd = false;
            _lastStartedTime = Game.Now;
            _showingControls = true;

            while (_controls != null && Vector2.Distance(_controls.GetGlobalTransform().Vector2, bottomMinusControlsHeight) > 5f)
            {
                var controlsPos = Vector2.Lerp(_controls.GetGlobalTransform().Vector2, bottomMinusControlsHeight, Game.DeltaTime);
                _controls.SetGlobalPosition(controlsPos);
                if (_player.HasVelocity() || 
                    Game.Input.Pressed(InputButtons.Attack))
                {
                    _anyInput = true;
                }
                yield return Wait.ForFrames(1);
            }

        }

        private IEnumerator<Wait> WaitForInput()
        {
            while (!_anyInput) 
            {
                if (_player.HasVelocity() || 
                    Game.Input.Pressed(InputButtons.Attack))
                {
                    _anyInput = true;
                }
                yield return Wait.ForFrames(1);
            }
        }

        private IEnumerator<Wait> HideControls()
        {
            var bottomCenter = _libraryAsset.Bounds.Center;
            bottomCenter.Y += (_libraryAsset.Bounds.Height / 2f) + 30f;

            while (_controls != null && Vector2.Distance(_controls.GetGlobalTransform().Vector2, bottomCenter) > 5f)
            {
                var controlsPos = Vector2.Lerp(_controls.GetGlobalTransform().Vector2, bottomCenter, Game.DeltaTime);
                _controls.SetGlobalPosition(controlsPos);
                yield return Wait.ForFrames(1);
            }
            _showingControls = false;
            _controls?.Destroy();
            BeginEnemySpawn();
        }

        private void BeginEnemySpawn()
        {
            var enemySpawnManager = new StateMachineComponent<EnemySpawnManager>(new EnemySpawnManager(_enemySpawnDataId));
            World.AddEntity(enemySpawnManager);
            var flakSpawnManager = new StateMachineComponent<FlakSpawnManager>(new FlakSpawnManager());
            World.AddEntity(flakSpawnManager);
        }

        private void DrawMessage(RenderContext render)
        {
            if (_showWindWarning)
            {
                float timeSinceAppeared = Game.Now - _lastStartedTime;
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
                bottomCenter.Y += _libraryAsset.Bounds.Height / 2f - 80f;
                Vector2 textPosition = bottomCenter;

                Game.Data.GetFont(100).Draw(
                    render.GameUiBatch,
                    _windWarningText,
                    position: textPosition,
                    alignment: new Vector2(0, 0),
                    scale: new Vector2(1, 1),
                    sort: 0.12f,
                    color: Palette.Colors[9],
                    strokeColor: null,
                    shadowColor: Palette.Colors[0],
                    maxWidth: maxWidth,
                    visibleCharacters: currentLength
                );
                
                if (_playNextSound < Game.Now && currentLength < _windWarningText.Length)
                {
                    BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().RadioBlip, Murder.Core.Sounds.SoundProperties.None);

                    _playNextSound = Game.Now + 0.12f;
                }
            }

            if (_showingControls)
            {
                float timeSinceAppeared = Game.Now - _lastStartedTime;
                int currentLength = Calculator.CeilToInt(Calculator.ClampTime(timeSinceAppeared, 2f /* dialog duration */) * _controlsText.Length);

                if (_reachedEnd || currentLength == _controlsText.Length)
                {
                    _reachedEnd = true;

                    currentLength = _controlsText.Length;
                }
                int maxWidth = 230;

                float dialogWidth = Math.Min(
                    Game.Data.GetFont(100).GetLineWidth(_controlsText.AsSpan().Slice(0, currentLength)), maxWidth);

                var bottomCenter = _libraryAsset.Bounds.Center;
                bottomCenter.X -= dialogWidth / 2f;
                bottomCenter.Y += _libraryAsset.Bounds.Height / 2f - 80f;
                Vector2 textPosition = bottomCenter;

                Game.Data.GetFont(100).Draw(
                    render.GameUiBatch,
                    _controlsText,
                    position: textPosition,
                    alignment: new Vector2(0, 0),
                    scale: new Vector2(1, 1),
                    sort: 0.12f,
                    color: Palette.Colors[9],
                    strokeColor: null,
                    shadowColor: Palette.Colors[0],
                    maxWidth: maxWidth,
                    visibleCharacters: currentLength
                );

                if (_playNextSound < Game.Now && currentLength < _controlsText.Length)
                {
                    BombsAwaySoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().RadioBlip, Murder.Core.Sounds.SoundProperties.None);

                    _playNextSound = Game.Now + 0.12f;
                }
            }
        }
    }
}