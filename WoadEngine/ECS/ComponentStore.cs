// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;

namespace WoadEngine.ECS;

#region Component Store

/// <summary>
/// Stores components of type <typeparamref name="T"/> using a "sparse set" layout.
/// 
/// This provices:
/// - Fast <c>Has</c>/<c>Get</c> by entity ID (0(1))
/// - Cache-friendly iteration across dense arrays
/// - Efficient removals via swap-remove (0(1))
/// 
/// Implementation details:
/// - <see cref="_sparse"/> maps <c>entityId</c> -> <c>denseIndex + 1</c> (0 means absent)
/// - <see cref="_denseEntities"/> stores entity IDs for each dense slot
/// - <see cref="_denseComponents"/> stores the component data in dense order
/// </summary>
/// <typeparam name="T">Component type. Typically a struct (data-only).</typeparam>
public sealed class ComponentStore<T> : IComponentStore where T : struct
{
    #region Fields
    /// <summary>
    /// Sparse map: entity ID -> dense index + 1 (0 means absent)
    /// </summary>
    private int[] _sparse;
    /// <summary>
    /// Dense list of entity IDs which currently have this component.
    /// Indexes align with <see cref="_denseComponents"/>.
    /// </summary>
    private int[] _denseEntities;
    /// <summary>
    /// Dense list of components. Indexes align with <see cref="_denseEntities"/>.
    /// </summary>
    private T[] _denseComponents;
    /// <summary>
    /// Number of active components stored (valid entires in dense arrays).
    /// </summary>
    private int _count;
    #endregion

    #region Properties
    /// <summary>
    /// Number of entities currently holding this component.
    /// </summary>
    public int Count => _count;
    /// <summary>
    /// Returns a span over the dense entity list (0..Count).
    /// Useful for fast iteration without allocations.
    /// </summary>
    public Span<int> DenseEntities => _denseEntities.AsSpan(0, _count);
    /// <summary>
    /// Returns a span over the dense component list (0..Count).
    /// Useful for fast iteration without allocations.
    /// </summary>
    public Span<T> DenseComponents => _denseComponents.AsSpan(0, _count);
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new store with an initial sparse capacity.
    /// </summary>
    /// <param name="initialEntityCapacity">
    /// Initial capacity of the sparse array, typically matching the world entity capacity.
    /// </param>
    public ComponentStore(int initialEntityCapacity = 1024)
    {
        _sparse = new int[initialEntityCapacity];
        _denseEntities = new int[128];
        _denseComponents = new T[128];
        _count = 0;
    }
    #endregion

    #region Capacity Management
    /// <summary>
    /// Ensures the sparse array can address entity IDs up to <paramref name="entityCapacity"/>.
    /// Call this when the world grows its entity capacity.
    /// </summary>
    public void EnsureEntityCapacity(int entityCapacity)
    {
        if (entityCapacity <= _sparse.Length) return;
        Array.Resize(ref _sparse, entityCapacity);
    }
    #endregion

    #region Membership / CRUD
    /// <summary>
    /// Returns true if the given entity ID currently has this component.
    /// </summary>
    public bool Has(int entityId)
    {
        return entityId >= 0 && entityId < _sparse.Length && _sparse[entityId] != 0;
    }

    /// <summary>
    /// Adds a component for <paramref name="entityId"/> if absent, and returns it by reference.
    /// If it already exists, returns the existing component by reference.
    /// </summary>
    /// <remarks>
    /// The returned <c>ref</c> allows callers to initialise the component without copying.
    /// </remarks>
    public ref T Add(int entityId)
    {
        if (Has(entityId))
            return ref Get(entityId);

        // Ensure sparse can address this ID
        if (entityId >= _sparse.Length)
            Array.Resize(ref _sparse, Math.Max(entityId + 1, _sparse.Length * 2));

        // Ensure dense arrays can fit another component.
        if (_count == _denseEntities.Length)
        {
            Array.Resize(ref _denseEntities, _denseEntities.Length * 2);
            Array.Resize(ref _denseComponents, _denseComponents.Length * 2);
        }

        int denseIndex = _count++;
        _denseEntities[denseIndex] = entityId;
        _denseComponents[denseIndex] = default;

        // Store denseIndex+1 so that 0 is reserved for "absent";
        _sparse[entityId] = denseIndex + 1;

        return ref _denseComponents[denseIndex];
    }

    /// <summary>
    /// Gets the component for <paramref name="entityId"/> by reference.
    /// Throws if the entity does not have this component.
    /// </summary>
    public ref T Get(int entityId)
    {
        int slot = _sparse[entityId];
        if (slot == 0) throw new InvalidOperationException($"Entity {entityId} does not have component {typeof(T).Name}");
        return ref _denseComponents[slot - 1];
    }

    /// <summary>
    /// Removes the component for <paramref name="entityId"/> if it exists.
    /// </summary>
    /// <remarks>
    /// uses swap-remove to keep dense arrays packed:
    /// - Move the last element into the removed slot
    /// - Fix up the moved entity's sparse mapping
    /// - Decrement count
    /// </remarks>
    public void Remove(int entityId)
    {
        int slot = _sparse[entityId];
        if (slot == 0) return;

        int denseIndex = slot - 1;
        int lastIndex = _count - 1;

        // If we're not removing the last item, swap the last item into the removed slot.
        if (denseIndex != lastIndex)
        {
            int movedEntity = _denseEntities[lastIndex];
            _denseEntities[denseIndex] = movedEntity;
            _denseComponents[denseIndex] = _denseComponents[lastIndex];

            // Update the sparse mapping for the moved entity.
            _sparse[movedEntity] = denseIndex + 1;
        } 

        // Clear the last slot.
        _denseEntities[lastIndex] = default;
        _denseComponents[lastIndex] = default;
        _count--;

        // Mark entity as not having this component.
        _sparse[entityId] = 0;
    }

    void IComponentStore.EnsureEntityCapacity(int entityCapacity) => EnsureEntityCapacity(entityCapacity);

    void IComponentStore.RemoveEntity(int entityId) => Remove(entityId);

    void IComponentStore.AddDefault(int entityId)
    {
        _ = Add(entityId);
    }

    void IComponentStore.SetBoxed(int entityId, object value)
    {
        ref var c = ref Add(entityId);
        c = (T)value;
    }
    #endregion

}
#endregion