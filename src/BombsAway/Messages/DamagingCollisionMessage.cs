using Bang.Components;
using System.Numerics;

namespace BombsAway.Messages
{
    /// <summary>
    /// Message used to relay collisions that apply damage
    /// </summary>
    public readonly struct DamagingCollisionMessage : IMessage
    {
        public readonly int DamageDealt = 1;
        public readonly int OtherEntityId = -1;
        public readonly Vector2 Center = Vector2.Zero;

        public DamagingCollisionMessage(int damageDealt, int otherEntityId, Vector2 center) : this()
        {
            OtherEntityId = otherEntityId;
            DamageDealt = damageDealt;
            Center = center;
        }
    }
}
