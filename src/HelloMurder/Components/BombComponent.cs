

using Bang.Components;
using System.Numerics;

namespace HelloMurder.Components
{
    public readonly struct BombComponent : IComponent
    {
        public readonly float TimeToTarget = 3f;
        public BombComponent() { }
        public BombComponent(float timeToTarget)
        {
            TimeToTarget = timeToTarget;
        }
    }
}
