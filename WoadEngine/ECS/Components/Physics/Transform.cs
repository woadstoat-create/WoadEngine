// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Physics;

#region Transform Component

/// <summary>
/// Basic 3D transform component (position, rotation, scale).
///
/// This is a data-only ECS component intended to be consumed by systems such as:
/// - Movement / physics integration (updates <see cref="Position"/>)
/// - Render systems (build world matrices for meshes/sprites/billboards)
/// - Parenting / hierarchy systems (optional, if you add them later)
///
/// Note: This is a *world-space* transform unless you introduce parent/child relationships.
/// </summary>
/// <remarks>
/// MonoGame conventions:
/// - <see cref="Vector3"/> is typically used for positions/scales.
/// - <see cref="Quaternion"/> is used for rotation to avoid gimbal lock and for smooth interpolation.
///
/// Keep this component free of behaviour (no Update methods); behaviour belongs in systems.
/// </remarks>
public struct Transform
{
    #region Fields
    /// <summary>
    /// World-space position of the entity.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// World-space rotation of the entity, stored as a quaternion.
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// World-space scale of the entity. <see cref="Vector3.One"/> means "no scale".
    /// </summary>
    public Vector3 Scale;
    #endregion

    #region Factory

    /// <summary>
    /// Creates a transform with sensible defaults:
    /// position = (0,0,0), rotation = identity, scale = (1,1,1).
    /// </summary>
    /// <param name="pos">Optional initial position. Defaults to <see cref="Vector3.Zero"/>.</param>
    /// <param name="rot">Optional initial rotation. Defaults to <see cref="Quaternion.Identity"/>.</param>
    /// <param name="scale">Optional initial scale. Defaults to <see cref="Vector3.One"/>.</param>
    /// <returns>A newly initialised <see cref="Transform"/> value.</returns>
    /// <remarks>
    /// This is declared <c>static</c> because it does not depend on an existing instance.
    /// Using a factory helps avoid uninitialised struct defaults (e.g. Scale = (0,0,0)).
    /// </remarks>
    public static Transform Create(Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
    {
        return new Transform
        {
            Position = pos ?? Vector3.Zero,
            Rotation = rot ?? Quaternion.Identity,
            Scale = scale ?? Vector3.One
        };
    }
    #endregion
}
#endregion