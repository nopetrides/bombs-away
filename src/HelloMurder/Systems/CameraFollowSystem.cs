using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using HelloMurder.Components;
using Murder.Utilities;
using Murder.Core.Graphics;
using Murder.Core;

namespace HelloMurder.Systems;

// Filtering the entities to only have the "PlayerComponent"
[Filter(typeof(PlayerComponent))]
internal class CameraFollowSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        // Making sure you have a player
        if (!context.HasAnyEntity)
            return;

        // Get current world, and grab that camera
        Camera2D camera = ((MonoWorld)context.World).Camera;

        // Get the context.Entity (we expect our player, since we filtered it!)
        // Get it's GlobalTransform and set our camera position
        camera.Position = context.Entity.GetGlobalTransform().Vector2;
    }
}