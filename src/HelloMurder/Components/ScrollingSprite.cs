using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace HelloMurder.Components
{
    public readonly struct ScrollingSprite : IComponent
    {
        [SpriteBatchReference]
        public readonly int TargetSpriteBatch = Batches2D.GameplayBatchId;

        public readonly Point Size = new Point(0, 3);
        public readonly Vector2 Speed = new Vector2(0, 100f);
        public readonly bool Flip = false;

        [Slider(0, 1f)]
        public readonly float Sort = 0f;

        [GameAssetId<SpriteAsset>]
        public readonly Guid Sprite = Guid.Empty;

        public ScrollingSprite()
        {
        }
    }
}
