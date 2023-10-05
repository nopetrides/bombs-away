using Bang.Components;

namespace HelloMurder.Messages
{
    /// <summary>
    /// Message used to relay collisions that apply damage
    /// </summary>
    public readonly struct DamagingCollisionMessage : IMessage
    {
        public readonly int DamageDealt = 1;
        public readonly int OtherEntityId = -1;

        public DamagingCollisionMessage(int damageDealt, int otherEntityId) : this()
        {
            OtherEntityId = otherEntityId;
            DamageDealt = damageDealt;
        }
    }
}
