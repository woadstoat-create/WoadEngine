namespace WoadEngine.ECS;

public interface IComponentStore
{
    void EnsureEntityCapacity(int entityCapacity);
    void RemoveEntity(int entityId);
    void AddDefault(int entityId);
    void SetBoxed(int entityId, object value);
}