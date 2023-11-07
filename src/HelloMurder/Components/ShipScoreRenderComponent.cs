using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace HelloMurder.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct ShipScoreRenderComponent : IComponent
    {
        public readonly Action<RenderContext, Entity> Draw;

        public readonly int ScoreValue;

        public readonly Vector2 ShipKilledPosition;

        public readonly float TextAlpha;

        public ShipScoreRenderComponent(Action<RenderContext, Entity> draw, int points, Vector2 shipPosition, float textAlpha)
        {
            Draw = draw;
            ScoreValue = points;
            ShipKilledPosition = shipPosition;
            TextAlpha = textAlpha;
        }
    }
}
