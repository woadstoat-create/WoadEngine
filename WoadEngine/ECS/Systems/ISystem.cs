// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

namespace WoadEngine.ECS.Systems;

#region Update System Interface
/// <summary>
/// ECS update system interface.
///
/// Update systems advance simulation state. They typically:
/// - read and write component data
/// - spawn/despawn entities (preferably via a command buffer)
/// - apply gameplay rules, AI, physics integration, etc.
///
/// In MonoGame terms, these systems should be called from <c>Game.Update</c>
/// (or from your Scene/Engine update pipeline).
/// </summary>
/// <remarks>
/// Design intent:
/// - Update systems should not perform rendering or depend on rendering services.
/// - Keep update code deterministic where possible (helps debugging and testing).
/// - Structural changes (create/destroy/add/remove components) are best deferred until
///   after iteration to avoid invalidating queries (command buffer pattern).
/// </remarks>
public interface ISystem
{
    /// <summary>
    /// Updates the simulation for the current frame/tick.
    /// </summary>
    /// <param name="world">The ECS world/registry containing entities and components.</param>
    /// <param name="dt">Delta time in seconds since the last update call.</param>
    void Update(World world, float dt);
}
#endregion