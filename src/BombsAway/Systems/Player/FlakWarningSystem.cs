using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using BombsAway.Components;
using BombsAway.Messages;
using Murder;
using Murder.Services;
using Murder.Utilities;

namespace BombsAway.Systems.Player
{
    [Filter(typeof(PlayerComponent))]
    [Messager(typeof(FlakWarningMessage))]
    internal class FlakWarningSystem : IMessagerSystem
    {
        private Entity? _flakWarning;

        private float _lastWarning;
        private readonly float _duration = 1.5f;

        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (_flakWarning == null)
            {
                _flakWarning = entity.TryFetchChild("flak_warning");
                _flakWarning?.Unparent();
            }

            _flakWarning?.Activate();
            _flakWarning?.SetGlobalPosition(entity.GetGlobalTransform().Vector2);

            _lastWarning = Game.Now;

            CoroutineServices.RunCoroutine(world, FlakWarning(_lastWarning));
        }

        private IEnumerator<Wait> FlakWarning(float warningTime)
        {
            yield return Wait.ForSeconds(_duration);
            if (_lastWarning == warningTime)
                _flakWarning?.Deactivate();
        }
    }
}
