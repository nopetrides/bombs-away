using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using BombsAway.Components;
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Services;
using Bang.Entities;
using Murder.Utilities;
using Murder;
using System.Numerics;
using Murder.Core.Geometry;

namespace BombsAway.Systems
{
    [Filter(typeof(ScrollingSprite), typeof(ITransformComponent))]
    internal class ScrollingImageSystem : IMurderRenderSystem, IUpdateSystem
    {
        float _distance = 0;
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                var position = e.GetGlobalTransform().ToVector2();
                var scroll = e.GetComponent<ScrollingSprite>();

                if (Game.Data.TryGetAsset<SpriteAsset>(scroll.Sprite) is SpriteAsset sprite)
                {
                    var size = sprite.Size;
                    var totalSize = size * scroll.Size;
                    var batch = render._spriteBatches[scroll.TargetSpriteBatch];


                    Vector2 offset = new Vector2((Game.Now * scroll.Speed.X) % size.X, (_distance * 100 * scroll.Speed.Y) % size.Y);

                    for (int x = 0; x < scroll.Size.X; x++)
                    {
                        for (int y = 0; y < scroll.Size.Y; y++)
                        {
                            RenderServices.DrawSprite(batch,
                                scroll.Sprite,
                                position + size * new Point(x, y) - totalSize / 2f + offset,
                                new DrawInfo(scroll.Sort)
                            {
                                FlippedHorizontal = scroll.Flip
                            });
                        }
                    }
                }
            }
        }

        public void Update(Context context)
        {
            var playerSpeed = context.World.TryGetUnique<PlayerSpeedComponent>()?.Speed ?? 0;
            _distance += Game.DeltaTime * playerSpeed;
        }
    }
}
