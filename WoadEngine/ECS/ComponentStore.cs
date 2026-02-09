

namespace WoadEngine.ECS;

public sealed class ComponentStore<T> where T : struct
{
    private int[] _sparse;
    private int[] _denseEntities;
    private T[] _denseComponents;
    private int _count;

    public int Count => _count;

    public ComponentStore(int initialEntityCapacity = 1024)
    {
        _sparse = new int[initialEntityCapacity];
        _denseEntities = new int[128];
        _denseComponents = new T[128];
        _count = 0;
    }

    public void EnsureEntityCapacity(int entityCapacity)
    {
        if (entityCapacity <= _sparse.Length) return;
        Array.Resize(ref _sparse, entityCapacity);
    }

    public bool Has(int entityId)
    {
        return entityId >= 0 && entityId < _sparse.Length && _sparse[entityId] != 0;
    }

    public ref T Add(int entityId)
    {
        if (Has(entityId))
            return ref Get(entityId);

        if (entityId >= _sparse.Length)
            Array.Resize(ref _sparse, Math.Max(entityId + 1, _sparse.Length * 2));

        if (_count == _denseEntities.Length)
        {
            Array.Resize(ref _denseEntities, _denseEntities.Length * 2);
            Array.Resize(ref _denseComponents, _denseComponents.Length * 2);
        }

        int denseIndex = _count++;
        _denseEntities[denseIndex] = entityId;
        _denseComponents[denseIndex] = default;

        _sparse[entityId] = denseIndex + 1;
        return ref _denseComponents[denseIndex];
    }

    public ref T Get(int entityId)
    {
        int slot = _sparse[entityId];
        if (slot == 0) throw new InvalidOperationException($"Entity {entityId} does not have component {typeof(T).Name}");
        return ref _denseComponents[slots - 1];
    }

    public void Remove(int entityId)
    {
        int slot = _sparse[entityId];
        if (slot == 0) return;

        int denseIndex = slot - 1;
        int lastIndex = _count - 1;

        if (denseIndex != lastIndex)
        {
            int movedEntity = _denseEntities[lastIndex];
            _denseEntities[denseIndex] = movedEntity;
            _denseComponents[denseIndex] = _denseComponents[lastIndex];
            _sparse[movedEntity] = denseIndex + 1;
        } 

        _denseEntities[lastIndex] = default;
        _denseComponents[lastIndex] = default;
        _count--;

        _sparse[entityId] = 0;
    }

    public Span<int> DenseEntities => _denseEntities.AsSpan(0, _count);
    public Span<T> DenseComponents => _denseComponents.AsSpan(0, _count);

}