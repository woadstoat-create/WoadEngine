// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

namespace WoadEngine.ECS;

/// <summary>
/// Lightweight, value-type handle identifying an entity inside an ECS <c>World</c>.
/// 
/// An <see cref="Entity"/> is intentionally NOT an object with behaviour or state.
/// It is a stable reference (while alive) used to look up component data in component stores.
/// </summary>
/// <remarks>
/// This handle uses common "index + generation" pattern:
/// - <see cref="ID"/> is an index into internal arrays (component sparse sets, generation table, etc.)
/// - <see cref="Gen"/> is a version number used to detect stale handles when IDs are recycled.
/// 
/// When an entity is destroyed, the world increments its generation for that ID.
/// Any older handled with the previous generation are considered invald.
/// </remarks>
public readonly struct Entity
{
    /// <summary>
    /// Entity slot/index allocated by the world. Used as the key into component storage.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// Generation/version for this entity ID. Used to validate that a handle is still alive.
    /// </summary>
    public readonly int Gen;

    /// <summary>
    /// Creates an entity handle. Typically constructed by the ECS world/entity manager.
    /// </summary>
    /// <param name="id">Entity slot/index.</param>
    /// <param name="gen">Entity generation/version at time of creation.</param>
    public Entity(int id, int gen) { ID = id; Gen = gen; }

    public override string ToString() => $"Entity({ID}:{Gen})";
}