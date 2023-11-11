using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using BombsAway.Components;
using Murder.Components;
using Murder.Helpers;
using Murder.Utilities;

namespace BombsAway.Systems.Player
{
    [Filter(typeof(MoveToPlayerComponent), typeof(AgentComponent))]
    public class MoveToPlayerSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            var player = context.World.TryGetUniqueEntity<PlayerComponent>();
            if (player == null) return;
            var playerPos = player.GetComponent<PositionComponent>().ToVector2();
            foreach (var e in context.Entities)
            {
                var ePos = e.GetComponent<PositionComponent>().ToVector2();
                var dirToPlayer = (playerPos - ePos).Normalized();
                e.SetAgentImpulse(dirToPlayer);
            }
        }
    }
}
