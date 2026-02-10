// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.ECS.Components.Rendering;

#region Camera Component
/// <summary>
/// 3D camera component (view/projection settings + cached matrices).
///
/// Intended usage:
/// - Attach <see cref="Camera"/> + <c>Transform</c> to an entity.
/// - A CameraSystem computes <see cref="View"/> and <see cref="Projection"/> each frame
///   from the entity transform and camera settings.
/// - Render systems read the active camera matrices when drawing.
/// </summary>
/// <remarks>
/// Design notes:
/// - The camera settings (FOV, near/far, viewport, etc.) live here.
/// - The computed matrices are cached here for convenience.
/// - "Active" selection can be done via <see cref="IsActive"/> or a separate tag component.
/// </remarks>
public struct Camera
{
    #region Settings
    /// <summary>
    /// If true, this camera is eligible to be used as the current view camera.
    /// If multiple cameras are active, the engine should define a selection rule.
    /// </summary>
    public bool IsActive;

    /// <summary>
    /// Vertical field-of-view in radians (MonoGame expects radians).
    /// Common values: ~0.785 (45°), ~1.047 (60°).
    /// </summary>
    public float FieldOfView;

    /// <summary>
    /// Near clipping plane distance.
    /// </summary>
    public float NearPlane;

    /// <summary>
    /// Far clipping plane distance.
    /// </summary>
    public float FarPlane;

    /// <summary>
    /// Aspect ratio (width/height). Usually derived from viewport each frame.
    /// Kept here so you can override it if needed (split-screen, render target, etc.).
    /// </summary>
    public float AspectRatio;

    /// <summary>
    /// If true, the system should update <see cref="AspectRatio"/> from the current viewport.
    /// </summary>
    public bool UseViewportAspect;

    /// <summary>
    /// Optional render layer mask for what this camera should see (bitmask).
    /// Your render systems decide how to use this (e.g. compare against MeshRenderer.RenderLayer).
    /// </summary>
    public uint CullingMask;
    #endregion

    #region Cached Outputs
    /// <summary>
    /// Cached view matrix computed by a camera system.
    /// </summary>
    public Matrix View;

    /// <summary>
    /// Cached projection matrix computed by a camera system.
    /// </summary>
    public Matrix Projection;
    #endregion

    #region Factory
    /// <summary>
    /// Creates a camera with sensible defaults (60° FOV, near 0.1, far 1000).
    /// Aspect ratio should be set by the camera system if <see cref="UseViewportAspect"/> is true.
    /// </summary>
    public static Camera Create(bool active = true)
    {
        return new Camera
        {
            IsActive = active,
            FieldOfView = MathHelper.ToRadians(60f),
            NearPlane = 0.1f,
            FarPlane = 1000f,
            AspectRatio = 16f / 9f,
            UseViewportAspect = true,
            CullingMask = 0xFFFF_FFFF,

            View = Matrix.Identity,
            Projection = Matrix.Identity
        };
    }
    #endregion
}

#endregion