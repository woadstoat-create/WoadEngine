namespace WoadEngine.ECS.Systems;

public interface IRenderSystem
{
    void Draw(World world, float dt);
}