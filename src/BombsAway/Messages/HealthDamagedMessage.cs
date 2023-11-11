using Bang.Components;

namespace BombsAway.Messages
{
    public readonly struct HealthDamagedMessage : IMessage
    {
        public readonly int DamageDealt;
        public HealthDamagedMessage(int damageDealt) : this()
        {
            DamageDealt = damageDealt;
        }
    }
}
