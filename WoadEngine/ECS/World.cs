// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WoadEngine.ECS;

#region World

/// <summary>
/// ECS "World" / registry: owns entity lifetime and component storage.
/// 
/// Responsibilities:
/// - Allocates and recycles entity IDs
/// - Tracks entity generations (stale handle detection)
/// - Owns component stores (one <see cref="ComponentStore{T}"/> per component type)
/// - Provides types componend CRUD operations (Add/Has/Get/Remove)
/// 
/// Notes: 
/// - Entties are lightweight handles (<see cref="Entity"/>) and are validated via ID + generation.
/// - Component storage is maintained in per-type stores, keyed by entity ID.
/// </summary>
/// <remarks>
/// This implementation uses reflection to call <c>EnsureEntityCapacity</c> and <c>Remove</c> on stores of unknown generic type.
/// This is acceptable for now, though we may want to elminate reflection later: 
/// - Track stores via a non-generic interface (e.g. <c>IComponentStore</c>)
/// - or keep a per-entity list/mask of attached component types for faster destroy.
/// </remarks>
public sealed class World
{
    #region Fields
    /// <summary>
    /// Generation/version per entity ID. Incremented on destory to invalidate old handles.
    /// </summary>
    private int[] _generations;
    /// <summary>
    /// Pool of recyclable entity IDs.
    /// </summary>
    private Stack<int> _freeIds;
    /// <summary>
    /// Next entity ID to allocate if the free list is empty.
    /// </summary>
    private int _nextId;

    /// <summary>
    /// Component stores, keyed by component type <see cref="Type"/>
    /// Each store instnace is a <see cref="ComponentStore{T}"/> for some T.  
    /// </summary>
    private readonly Dictionary<Type, object> _stores = new();
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new world with an initial capacity for entities.
    /// </summary>
    /// <param name="initialEntityCapacity">
    /// Initial size of the generation table and sparse arrays inside component stores.
    /// This does not limit the world; it grows automatically as needed.
    /// </param>
    public World(int initialEntityCapacity = 1024)
    {
        _generations = new int[initialEntityCapacity];
        _freeIds = new Stack<int>();
        _nextId = 0;
    }
    #endregion

    #region Entity Lifescycle
    /// <summary>
    /// Creates a new entity handle
    /// </summary>
    /// <remarks>
    /// Entities are allocated either by reusing an ID from the free list or by taking the next new ID.
    /// The returned handle includes the current generation for that ID.
    /// 
    /// If the world grows the generation table, existing component stores are asked to resize
    /// their sparse arrays to match, ensuring all stores can address new entity IDs.
    public Entity CreateEntity()
    {
        // Reuse IDs where possible to keep arrays dense and memory predictable
        int id = _freeIds.Count > 0 ? _freeIds.Pop() : _nextId++;

        // Ensure the generation table can address this entity ID.
        if (id >= _generations.Length)
            Array.Resize(ref _generations, Math.Max(id + 1, _generations.Length * 2));
        
        // Ensure existing stores can address all current entity IDs
        foreach (var storeObj in _stores.Values)
            EnsureStoreEntityCapacity(storeObj, _generations.Length);
        
        return new Entity(id, _generations[id]);
    }

    /// <summary>
    /// Return true if the entity handle refers to a currently alive entity.
    /// </summary>
    /// <remarks>
    /// Validates both: 
    /// - the ID is within bounds
    /// - the generation matches the world's generation for that ID
    /// 
    /// If an entity is destroyed and its ID recycled, the generation increments, causing old handles to fail this check.
    /// </remarks>
    public bool IsAlive(Entity e)
    {
        return e.ID >= 0 && e.ID < _generations.Length && _generations[e.ID] == e.Gen;
    }

    /// <summary>
    /// Destroys an entity (if alive), removing all components and invalidating its handle.
    /// </summary>
    /// <remarks>
    /// This implementation removes the entity from every component store (reflection-based),
    /// then increments the generation and recycles the ID.
    /// 
    /// In a more advanced ECS you would typically defer structural changes via a command buffer, especially if destruction can occur during iteration.
    /// </remarks>
    public void DestroyEntity(Entity e)
    {
        if (!IsAlive(e)) return;

        // Remove components from all stores
        foreach (var storeObj in _stores.Values)
            RemoveEntityFromStore(storeObj, e.ID);

        // Invalidate stale handles for this ID and recycle it.
        _generations[e.ID]++;
        _freeIds.Push(e.ID);
    }

    /// <summary>
    /// Tries to destroy an entity by its raw ID.
    /// </summary>
    /// <returns>True if the entity was alive and got destroyed; otherwise false.</returns>
    public bool TryDestroyById(int id)
    {
        if (id < 0 || id >= _generations.Length)
            return false;

        var e = new Entity(id, _generations[id]);
        if (!IsAlive(e))
            return false;

        DestroyEntity(e);
        return true;
    }
    #endregion

    #region Component API
    /// <summary>
    /// Adds component <typeparamref name="T"/> to an entity and returns it by reference.
    /// </summary>
    /// <remarks>
    /// The returned <c>ref</c> lets callers initialise the component without copying.
    /// </remarks>
    public ref T Add<T>(Entity e) where T : struct
    {
        ValidateAlive(e);
        return ref GetStore<T>().Add(e.ID);
    }

    /// <summary>
    /// Returns true if the entity currently has component <typeparamref name="T"/>.
    /// </summary>
    public bool Has<T>(Entity e) where T : struct
    {
        if (!IsAlive(e)) return false;
        return GetStore<T>().Has(e.ID);
    }

    /// <summary>
    /// Gets component <typeparamref name="T"/> from an entity by reference.
    /// Throws if missing or the entity is not alive.
    /// </summary>
    public ref T Get<T>(Entity e) where T : struct
    {
        ValidateAlive(e);
        return ref GetStore<T>().Get(e.ID);
    }

    /// <summary>
    /// Removes component <typeparamref name="T"/> from an entity if present.
    /// </summary>
    public void Remove<T>(Entity e) where T : struct
    {
        if (!IsAlive(e)) return;
        GetStore<T>().Remove(e.ID);
    }

    /// <summary>
    /// Returns the component store for <typeparamref name="T"/>, creating it if needed.
    /// </summary>
    /// <remarks>
    /// Stores are created lazily so you don't pay upfront allocations for component types
    /// you never use in a given game/session.
    /// </remarks>
    public ComponentStore<T> GetStore<T>() where T : struct
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var existing))
            return (ComponentStore<T>)existing;

        // New stores should match current world capacity so sparse indexing is valid.
        var store = new ComponentStore<T>(_generations.Length);
        _stores[type] = store;
        return store;
    }
    #endregion

    #region Validation
    /// <summary>
    /// Throws if the entity handle is not alive.
    /// </summary>
    private void ValidateAlive(Entity e)
    {
        if (!IsAlive(e)) throw new InvalidOperationException($"Entity handle is not alive: {e}");
    }
    #endregion

    #region Store Reflection Helpers

    /// <summary>
    /// Invokes <c>EnsureEntityCapacity(int)</c> on a component store of unknown generic type.
    /// </summary>
    /// <remarks>
    /// Used when the world grows its entity capacity so that all sparse arrays remain valid.
    /// </remarks>
    private static void EnsureStoreEntityCapacity(object storeObj, int capacity)
    {
        var method = storeObj.GetType().GetMethod(nameof(ComponentStore<int>.EnsureEntityCapacity));
        method?.Invoke(storeObj, new object[] { capacity });
    }

    /// <summary>
    /// Invokes <c>Remove(int)</c> on a component store of unknown generic type.
    /// </summary>
    /// <remarks>
    /// Used during entity destruction to remove that entity from all component stores.
    /// </remarks>
    private static void RemoveEntityFromStore(object storeObj, int entityId)
    {
        var method = storeObj.GetType().GetMethod(nameof(ComponentStore<int>.Remove));
        method?.Invoke(storeObj, new Object[] { entityId});
    }
    #endregion
}
#endregion