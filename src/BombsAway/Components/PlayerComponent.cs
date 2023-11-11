using Bang.Components;
using Murder.Attributes;

namespace BombsAway.Components
{
    [Unique]
    [DoNotPersistEntityOnSave]
    public readonly struct PlayerComponent : IComponent
    {
        public PlayerComponent()
        {
        }
    }
}

