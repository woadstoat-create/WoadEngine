// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using WoadEngine;
using WoadEngine.ECS.Components.Rendering;
using WoadEngine.ECS.Components.Physics;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Systems.Rendering;

#region Sprite Render System

public sealed class SpriteRenderSystem : IRenderSystem
{
    public void Draw(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var sprites = world.GetStore<SpriteRenderer>();

        if (transforms.Count <= sprites.Count)
        {
            var ents = transforms.DenseEntities;
            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!sprites.Has(id)) continue;

                ref var t = ref transforms.DenseComponents[i];
                ref var s = ref sprites.Get(id);

                s.Region.Draw(Core.SpriteBatch, new Vector2(t.Position.X, t.Position.Y), s.Color, t.Rotation.Z, s.Origin, new Vector2(t.Scale.X, t.Scale.Y), s.Effect, s.Layer);
            }
        }
        else
        {
            var ents = sprites.DenseEntities;
            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!transforms.Has(id)) continue;

                ref var s = ref sprites.DenseComponents[i];
                ref var t = ref transforms.Get(id);

                s.Region.Draw(Core.SpriteBatch, new Vector2(t.Position.X, t.Position.Y), s.Color, t.Rotation.Z, s.Origin, new Vector2(t.Scale.X, t.Scale.Y), s.Effect, s.Layer);
            }
        }
    }
}

#endregion