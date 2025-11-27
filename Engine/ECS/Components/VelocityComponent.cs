using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class VelocityComponent : IComponent
    {
        public Vector2 Velocity;
        public float Multiplier = 1.0f;
    }
}