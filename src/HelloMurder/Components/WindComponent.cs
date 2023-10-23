using Bang.Components;
using System.Numerics;

namespace HelloMurder.Components
{
    public readonly struct WindComponent : IComponent
    {
        public readonly Vector2 WindVector = Vector2.Zero;
        public WindComponent() { }
        public WindComponent(Vector2 wind)
        {
            WindVector = wind;
        }
    }
}
