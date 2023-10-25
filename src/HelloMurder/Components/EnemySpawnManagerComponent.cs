using Bang.Components;
using Murder.Utilities.Attributes;

namespace HelloMurder.Components
{
    [Unique]
    [RuntimeOnly]
    public readonly struct EnemySpawnManagerComponent : IComponent
    {
    }
}
