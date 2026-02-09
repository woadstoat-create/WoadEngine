namespace WoadEngine.ECS;

public readonly struct Entity
{
    public readonly int ID;
    public readonly int Gen;
    public Entity(int id, int gen) { ID = id; Gen = gen; }
    public override string ToString() => $"Entity({ID}:{Gen})";
}