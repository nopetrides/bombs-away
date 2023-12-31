﻿using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using Bang.Entities;
using Murder;

namespace BombsAway.Systems
{
    [DoNotPause]
    [Filter(kind: ContextAccessorKind.Read, typeof(FadeScreenComponent))]
    internal class SimpleFadeScreenSystem : IRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                FadeScreenComponent fade = e.GetFadeScreen();

                float current = 0;
                float fullTime = (Game.NowUnscaled - fade.StartedTime);
                float ratio = Math.Min(1, (fullTime - Game.FixedDeltaTime) / fade.Duration);

                switch (fade.Fade)
                {
                    case FadeType.In:
                        current = ratio;
                        break;

                    case FadeType.Out:
                        current = 1f - ratio;
                        break;

                    case FadeType.Flash:
                        current = 1 - Math.Abs(ratio - 0.5f) * 2;
                        break;
                }

                RenderServices.DrawRectangle(render.UiBatch,
                    new(0, 0, render.ScreenSize.X, render.ScreenSize.Y),
                    fade.Color * Calculator.Clamp01(Ease.CubeInOut(current)), 0.0001f); // Not zero because the letterbox borders have priority

                if (fade.DestroyAfterFinished && (fullTime > fade.Duration + Game.FixedDeltaTime))
                {
                    e.Destroy();
                }
            }
        }
    }
}
