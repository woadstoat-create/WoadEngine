// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Physics;

#region Collider2D Component

/// <summary>
/// 2D axis-aligned bounding box (AABB) collider component.
///
/// This is a data-only component intended to be consumed by collision/physics systems,
/// typically by querying entities that have <c>Transform2D + Collider2D</c>.
/// </summary>
/// <remarks>
/// Coordinate conventions (recommended):
/// - <see cref="Rect"/> represents the collider bounds in *local space* (typically width/height),
///   and its X/Y may be treated as local offset as well.
/// - <see cref="Offset"/> is an additional local offset applied when computing world bounds.
/// - Actual world-space collision bounds are usually computed as:
///   <c>worldRect = Rect translated by (Transform.Position + Offset)</c>.
///
/// Note: <see cref="Rectangle"/> uses integers, which is fine for pixel-perfect 2D,
/// but if your transforms are float-based you’ll need a consistent rounding policy
/// (floor/round/ceil) in your collision system.
/// </remarks>
public struct Collider2D
{
    #region Fields
    /// <summary>
    /// Local-space rectangle defining collider size (and optionally local offset).
    /// </summary>
    public Rectangle Rect;

    /// <summary>
    /// Additional local-space offset applied when converting the collider to world space.
    /// Useful when the entity origin is not at the top-left of its collision bounds.
    /// </summary>
    public Vector2 Offset;
    #endregion

    #region Factory
    /// <summary>
    /// Creates a <see cref="Collider2D"/> with the given bounds and optional offset.
    /// </summary>
    /// <param name="x">Local X (usually 0 unless you encode local offset in the rectangle).</param>
    /// <param name="y">Local Y (usually 0 unless you encode local offset in the rectangle).</param>
    /// <param name="w">Width in pixels/units.</param>
    /// <param name="h">Height in pixels/units.</param>
    /// <param name="offset">Additional local offset applied when computing world bounds.</param>
    /// <returns>A newly initialised collider component.</returns>
    public static Collider2D Create(int x, int y, int w, int h, Vector2? offset = null)
    {
        return new Collider2D
        {
            Rect = new Rectangle(x, y, w, h),
            Offset = offset ?? Vector2.Zero
        };
    }
    #endregion
}
#endregion