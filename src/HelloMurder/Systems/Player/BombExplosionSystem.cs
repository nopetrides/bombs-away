using Bang.Contexts;
using Bang.Systems;
using HelloMurder.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;

namespace HelloMurder.Systems.Player
{
    [Filter(typeof(BombComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DestroyOnAnimationCompleteComponent))]
    public class BombExplosionSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach(var e in context.Entities)
            {
                var sprite = e.GetSprite();
                if (sprite.CurrentAnimation == "damage")
                {
                    IShape shape = new CircleShape(12, new Point(0, 0));
                    var layer = CollisionLayersBase.TRIGGER & CollisionLayersBase.ACTOR & CollisionLayersBase.HITBOX;
                    ColliderComponent col = new ColliderComponent(shape, layer, Color.Blue);
                    e.SetCollider(col);
                    e.SetDestroyOnAnimationComplete(false);
                }
            }
        }
    }
}
