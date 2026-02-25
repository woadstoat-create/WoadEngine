// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Physics;

#region Collider3DBox

/// <summary>
/// 3D axis-aligned bounding box collider (AABB).
/// Local-space bounds are translated to world-space using the entity Transform.
/// </summary>
public struct Collider3DBox
{
    /// <summary>
    /// Local-space center of the box relative to the entity origin.
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// Local-space half extents (half-width, half-height, half-depth).
    /// Example: size (2,4,2) -> extents (1,2,1).
    /// </summary>
    public Vector3 Extents;

    /// <summary>
    /// Collision layer/mask hooks (optional).
    /// Keep these even if you don’t implement filtering yet; they’re useful later.
    /// </summary>
    public int Layer;
    public int Mask;

    /// <summary>
    /// Creates an AABB collider from a full size (width/height/depth).
    /// </summary>
    public static Collider3DBox Create(Vector3 size, Vector3? center = null, int layer = 0, int mask = ~0)
    {
        return new Collider3DBox
        {
            Center = center ?? Vector3.Zero,
            Extents = size * 0.5f,
            Layer = layer,
            Mask = mask
        };
    }
}
#endregion