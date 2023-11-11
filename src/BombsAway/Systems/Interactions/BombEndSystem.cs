using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using BombsAway.Components;
using Murder.Messages;

namespace BombsAway.Systems.Interactions
{
    [Filter(typeof(BombComponent))]
    [Messager(typeof(FatalDamageMessage))]
    internal class BombEndSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            entity.Destroy();
        }
    }
}
