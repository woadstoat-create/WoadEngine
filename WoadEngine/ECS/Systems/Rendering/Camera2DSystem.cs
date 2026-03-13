using Microsoft.Xna.Framework;
using WoadEngine;
using WoadEngine.ECS;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;

namespace WoadEngine.ECS.Systems.Rendering;

public sealed class Camera2DSystem : ISystem
{
    public void Update(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var cameras = world.GetStore<Camera2D>();

        int viewportWidth = Core.GraphicsDevice.Viewport.Width;
        int viewportHeight = Core.GraphicsDevice.Viewport.Height;

        var camEntities = cameras.DenseEntities;
        for (int i = 0; i < camEntities.Length; i++)
        {
            int entity = camEntities[i];

            if (!transforms.Has(entity))
                continue;

            ref var cam = ref cameras.DenseComponents[i];
            if (!cam.IsActive)
                continue;

            ref var tr = ref transforms.Get(entity);

            Vector2 origin = cam.Origin;
            if (origin == Vector2.Zero)
                origin = new Vector2(viewportWidth * 0.5f, viewportHeight * 0.5f);

            cam.View =
                Matrix.CreateTranslation(-tr.Position.X, -tr.Position.Y, 0f) *
                Matrix.CreateRotationZ(-cam.Rotation) *
                Matrix.CreateScale(cam.Zoom, cam.Zoom, 1f) *
                Matrix.CreateTranslation(origin.X, origin.Y, 0f);

            world.SetActiveCamera(entity);
            return;
        }
    }
}