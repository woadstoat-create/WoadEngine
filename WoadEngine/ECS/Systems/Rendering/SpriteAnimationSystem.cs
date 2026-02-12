// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using WoadEngine;
using WoadEngine.ECS.Components.Rendering;
using WoadEngine.ECS.Components.Physics;
using Microsoft.Xna.Framework;
using System;

namespace WoadEngine.ECS.Systems.Rendering;

#region Animated Sprite Render System
public sealed class SpriteAnimationSystem : ISystem
{
    public void Update(World world, float dt)
    {
        var sprites = world.GetStore<SpriteRenderer>();
        var animators = world.GetStore<SpriteAnimator>();

        if (sprites.Count <= animators.Count)
        {
            var ents = sprites.DenseEntities;
            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!animators.Has(id)) continue;

                ref var s = ref sprites.DenseComponents[i];
                ref var a = ref animators.Get(id);

                a.Elapsed += TimeSpan.FromSeconds((double)(new decimal(dt)));

                if (a.Elapsed >= a.Animation.Delay)
                {
                    a.Elapsed -= a.Animation.Delay;
                    a.CurrentFrame++;

                    if (a.CurrentFrame >= a.Animation.Frames.Count)
                    {
                        a.CurrentFrame = 0;
                    }

                    s.Region = a.Animation.Frames[a.CurrentFrame];
                }
            }
        }
    }
}
#endregion