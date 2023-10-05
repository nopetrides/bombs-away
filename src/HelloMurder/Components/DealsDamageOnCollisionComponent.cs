using Bang.Components;

namespace HelloMurder.Components
{
    /// <summary>
    /// Component that tracks how much damage is dealt by this entity
    /// </summary>
    public readonly struct DealsDamageOnCollisionComponent : IComponent
    {
        public readonly int Damage = 1;

        public DealsDamageOnCollisionComponent()
        {
        }

        public DealsDamageOnCollisionComponent(int damage)
        {
            Damage = damage;
        }
    }
}
