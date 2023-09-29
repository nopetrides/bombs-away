using Bang.Components;
using Murder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder.Components
{
    /// <summary>
    /// Component that tracks the current health of an entity
    /// </summary>
    [DoNotPersistEntityOnSave]
    public readonly struct HealthComponent : IComponent
    {
        public readonly int MaxHealth;
        public readonly int _health;
        public readonly int CurrentHealth => _health;
        public HealthComponent() 
        {
        }

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            _health = maxHealth;
        }

        private HealthComponent(int maxHealth, int health)
        {
            MaxHealth = maxHealth;
            _health = health;
        }

        internal HealthComponent Damage(int damagetaken) 
        { 
            return new HealthComponent(MaxHealth, Math.Max(_health - damagetaken, 0));
        }

        internal HealthComponent Heal(int healAmount)
        {
            return new HealthComponent(MaxHealth, Math.Min(_health + healAmount, MaxHealth));
        }
    }
}
