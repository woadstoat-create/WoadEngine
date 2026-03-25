// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using WoadEngine;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.Rendering;

namespace WoadEngine.ECS.Systems.Rendering;

public sealed class MeshRenderSystem : IRenderSystem
{
    private readonly ModelCache _modelCache = new();

    public void Draw(World world, float dt)
    {
        if (!world.TryGetActiveCamera(out int camId, out Camera cam, out Transform camTr))
            return;

        var transforms = world.GetStore<Transform>();
        var meshes = world.GetStore<MeshRenderer>();

        Core.GraphicsDevice.BlendState = BlendState.Opaque;
        Core.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Core.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        Core.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        for (int i = 0; i < meshes.Count; i++)
        {
            int entityId = meshes.DenseEntities[i];
            ref var meshRenderer = ref meshes.DenseComponents[i];

            if (!meshRenderer.Visible)
                continue;

            if (!transforms.Has(entityId))
                continue;

            ref var tr = ref transforms.Get(entityId);

            DrawModel(meshRenderer, tr, cam);
        }
    }

    private void DrawModel(in MeshRenderer meshRenderer, in Transform tr, in Camera cam)
    {
        if (string.IsNullOrWhiteSpace(meshRenderer.MeshId))
            return;

        Model model;
        try
        {
            model = _modelCache.Get(meshRenderer.MeshId);
        }
        catch
        {
            return;
        }

        Matrix worldMatrix =
            Matrix.CreateScale(tr.Scale) *
            Matrix.CreateFromQuaternion(tr.Rotation) *
            Matrix.CreateTranslation(tr.Position);

        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (Effect effect in mesh.Effects)
            {
                if (effect is BasicEffect basic)
                {
                    basic.World = worldMatrix;
                    basic.View = cam.View;
                    basic.Projection = cam.Projection;

                    basic.EnableDefaultLighting();
                    basic.PreferPerPixelLighting = false;

                    basic.DiffuseColor = meshRenderer.Color.ToVector3();
                    basic.Alpha = meshRenderer.Color.A / 255f;
                }
                else
                {
                    effect.Parameters["World"]?.SetValue(worldMatrix);
                    effect.Parameters["View"]?.SetValue(cam.View);
                    effect.Parameters["Projection"]?.SetValue(cam.Projection);
                }
            }

            mesh.Draw();
        }
    }
}