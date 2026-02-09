// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

namespace WoadEngine.ECS.Systems;

#region Render System Interface
/// <summary>
/// ECS render system interface.
///
/// Render systems convert the current world state into draw calls. They should:
/// - read component data (e.g., Transform + Sprite/Mesh)
/// - submit draw calls using the provided rendering context
///
/// In MonoGame terms, these systems should be called from <c>Game.Draw</c>
/// (or from your Scene/Engine draw pipeline).
/// </summary>
/// <remarks>
/// Important conventions:
/// - Render systems should avoid mutating simulation state.
/// - Avoid structural changes during draw (no create/destroy/add/remove components).
/// - If you need to trigger gameplay changes due to rendering (rare), enqueue
///   an event/command and apply it during Update.
///
/// Passing a <see cref="RenderContext"/> makes rendering dependencies explicit
/// and keeps systems decoupled from global engine singletons.
/// </remarks>
public interface IRenderSystem
{
    /// <summary>
    /// Draws the current state of the world for this frame.
    /// </summary>
    /// <param name="world">The ECS world/registry containing entities and components.</param>
    /// <param name="context">
    /// Render-time dependencies and per-pass state (SpriteBatch, GraphicsDevice, camera, etc.).
    /// </param>
    void Draw(World world, float dt);
}
#endregion