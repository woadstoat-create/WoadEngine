using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class ColliderComponent : IComponent
    {
        public Rectangle Collider;
        public bool IsTrigger;
    }
}