// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Physics;

#region Velocity Component

/// <summary>
/// 3D linear velocity component (units per second).
///
/// This is a data-only ECS component intended to be consumed by movement/physics systems,
/// typically by querying entities with <c>Transform + Velocity</c> and integrating:
/// <c>Transform.Position += Velocity.Value * dt</c>.
/// </summary>
/// <remarks>
/// Conventions:
/// - Velocity is expressed in world units per second.
/// - Acceleration, drag, max speed, etc. (if needed) are usually separate components
///   so systems can be composed cleanly.
/// </remarks>
public struct Velocity
{
    #region Fields
    
    /// <summary>
    /// Linear velocity in world units per second.
    /// </summary>
    public Vector3 Value;
    #endregion

    #region Factories
    /// <summary>
    /// Convenience factory for creating a velocity component.
    /// </summary>
    public static Velocity Create(Vector3 v)
    {
        return new Velocity { Value = v };
    }

    /// <summary>
    /// Convenience factory for a zero velocity component.
    /// </summary>
    public static Velocity Zero()
    {
        return new Velocity { Value = Vector3.Zero };
    }
    #endregion
}
#endregion