// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using WoadEngine.ECS;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;
using WoadEngine;

namespace WoadEngine.ECS.Systems.Rendering;

#region Camera System

/// <summary>
/// Updates camera matrices (View/Projection) from Transform + Camera settings.
///
/// This system:
/// - Finds an active camera entity (Camera.IsActive == true)
/// - Computes View from Transform (Position/Rotation)
/// - Computes Projection from camera parameters + aspect ratio
///
/// Notes:
/// - This version is update-time and does not directly depend on GraphicsDevice.
/// - You must provide the current viewport size/aspect via <see cref="SetViewport"/>.
/// </summary>
public sealed class CameraSystem : ISystem
{
    public void Update(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var cameras = world.GetStore<Camera>();

        if (cameras.Count <= transforms.Count)
        {
            var camEnts = cameras.DenseEntities;
            for (int i = 0; i < camEnts.Length; i++)
            {
                int id = camEnts[i];

                // Enusre entity also has a transform
                if (!transforms.Has(id)) continue;

                ref var cam = ref cameras.DenseComponents[i];
                if (!cam.IsActive) continue;

                ref var tr = ref transforms.Get(id);
                UpdateCameraMatrices(ref cam, ref tr);
                world.SetActiveCamera(id);

                // Remove return if we later want more than one active camera
                return;
            }
        }
        else
        {
            var trEnts = transforms.DenseEntities;
            for (int i = 0; i < trEnts.Length; i++)
            {
                int id = trEnts[i];

                if (!cameras.Has(id)) continue;

                ref var tr = ref transforms.DenseComponents[i];
                ref var cam = ref cameras.Get(id);
                if (!cam.IsActive) continue;

                UpdateCameraMatrices(ref cam, ref tr);
                world.SetActiveCamera(id);
                
                return;
            }
        }
    }

    private void UpdateCameraMatrices(ref Camera cam, ref Transform tr)
    {
        // Camera forward convention:
        // In MonoGame, Vector3.Forward is (0,0,-1) and Backward is (0,0,1).
        // We’ll treat "forward" as looking along -Z in local space.
        Vector3 forward = Vector3.Transform(Vector3.Forward, tr.Rotation);
        Vector3 up = Vector3.Transform(Vector3.Up, tr.Rotation);

        Vector3 position = tr.Position;
        Vector3 target = position + forward;

        cam.View = Matrix.CreateLookAt(position, target, up);

        float aspect = cam.UseViewportAspect
            ? (Core.GraphicsDevice.Viewport.Width / (float)Core.GraphicsDevice.Viewport.Height)
            : cam.AspectRatio;

        cam.AspectRatio = aspect;

        cam.Projection = Matrix.CreatePerspectiveFieldOfView(
            cam.FieldOfView,
            aspect,
            cam.NearPlane,
            cam.FarPlane
        );
    }
}
#endregion