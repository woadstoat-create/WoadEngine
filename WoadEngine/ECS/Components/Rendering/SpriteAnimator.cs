// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.Rendering;

namespace WoadEngine.ECS.Components.Rendering;

#region Animated Sprite Rendering Component
public struct SpriteAnimator
{
    public int CurrentFrame;
    public TimeSpan Elapsed;
    public Animation Animation;
}
#endregion