
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using BombsAway.Components;
using Murder.Core.Graphics;

namespace BombsAway.Systems
{
    [Filter(typeof(ShipScoreRenderComponent))]
    public class ShipScoreDrawRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                e.GetShipScoreRender().Draw(render, e);
            }
        }
    }
}
