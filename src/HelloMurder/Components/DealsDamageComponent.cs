using Bang.Components;

namespace HelloMurder.Components
{
    /// <summary>
    /// Component that tracks how much damage is dealt by this entity
    /// </summary>
    public readonly struct DealsDamageComponent : IComponent
    {
        public readonly int Damage = 1;

        public DealsDamageComponent()
        {
        }

        public DealsDamageComponent(int damage)
        {
            Damage = damage;
        }
    }
}
