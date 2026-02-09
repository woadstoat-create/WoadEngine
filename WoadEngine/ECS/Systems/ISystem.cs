namespace WoadEngine.ECS.Systems;

public interface ISystem
{
    void Update(World world, float dt);
}