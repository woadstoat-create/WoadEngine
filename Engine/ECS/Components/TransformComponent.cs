using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class TransformComponent : IComponent
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale = Vector2.One;
    }
}