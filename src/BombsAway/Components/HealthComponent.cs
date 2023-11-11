using Bang.Components;
using Murder.Attributes;

namespace BombsAway.Components
{
    /// <summary>
    /// Component that tracks the current health of an entity
    /// </summary>
    [DoNotPersistEntityOnSave]
    public readonly struct HealthComponent : IComponent
    {
        public readonly int MaxHealth;
        public readonly int Health;
        public HealthComponent()
        {
        }

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        private HealthComponent(int maxHealth, int health)
        {
            MaxHealth = maxHealth;
            Health = health;
        }

        internal HealthComponent Damage(int damagetaken)
        {
            return new HealthComponent(MaxHealth, Math.Max(Health - damagetaken, 0));
        }

        internal HealthComponent Heal(int healAmount)
        {
            return new HealthComponent(MaxHealth, Math.Min(Health + healAmount, MaxHealth));
        }
    }
}
