
namespace WoadEngine.ECS;

public sealed class World
{
    private int[] _generations;
    private Stack<int> _freeIds;
    private int _nextId;

    private readonly Dictionary<Type, object> _stores = new();

    public World(int initialEntityCapacity = 1024)
    {
        _generations = new int[initialEntityCapacity];
        _freeIds = new Stack<int>();
        _nextId = 0;
    }

    public Entity CreateEntity()
    {
        int id = _freeIds.Count > 0 ? _freeIds.Pop() : _nextId++;

        if (id >= _generations.Length)
            Array.Resize(ref _generations, Math.Max(id + 1, _generations.Length * 2));
        
        foreach (var storeObj in _stores.Values)
            EnsureStoreEntityCapacity(storeObj, generations.Length);
        
        return new Entity(id, _generations[id]);
    }

    public bool IsAlive(Entity e)
    {
        return e.Id >= 0 && e.Id < _generations.Length && _generations[e.Id] == e.Gen;
    }

    public void DestroyEntity(Entity e)
    {
        if (!IsAlive(e)) return;

        foreach (var storeObj in _stores.Values)
            RemoveEntityFromStore(storeObj, e.Id);

        _generations[e.Id]++;
        _freeIds.Push(e.Id);
    }

    public ref T Add<T>(Entity e) where T : struct
    {
        ValidateAlive(e);
        return ref GetStore<T>().Add(e.Id);
    }

    public bool Has<T>(Entity e) where T : struct
    {
        if (!IsAlive(e)) return false;
        return GetStore<T>().Has(e.Id);
    }

    public ref T Get<T>(Entity e) where T : struct
    {
        ValidateAlive(e);
        return ref GetStores<T>().Get(e.Id);
    }

    public void Remove<T>(Entity e) where T : struct
    {
        if (!IsAlive(e)) return;
        GetStore<T>().Remove(e.Id);
    }

    public ComponentStore<T> GetStore<T>() where T : struct
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var existing))
            return (ComponentStore<T>)existing;

        var store = new ComponentStore<T>(_generations.Length);
        _stores[type] = store;
        return store;
    }

    private void ValidateAlive(Entity e)
    {
        if (!IsAlive(e)) throw new InvalidOperationException($"Entity handle is not alive: {e}");
    }

    private static void EnsureStoreEntityCapacity(object storeObj, int capacity)
    {
        var method = storeObj.GetType().GetMethod(nameof(ComponentStore<int>.EnsureStoreEntityCapacity));
        method?.Invoke(storeObj, new object[] { capacity });
    }

    private static void RemoveEntityFromStore(object storeObj, int entityId)
    {
        var method = storeObj.GetType().GetMethod(nameof(ComponentStore<int>.Remove));
        method?.Invoke(storeObj, new Object[] { entityId});
    }
}