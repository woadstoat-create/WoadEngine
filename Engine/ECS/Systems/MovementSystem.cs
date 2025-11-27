using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class MovementSystem : IUpdateSystem
    {
        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var e in entities)
            {
                if (e.TryGetComponent<TransformComponent>(out var transform) &&
                    e.TryGetComponent<VelocityComponent>(out var velocity))
                {
                    transform.Position += velocity.Velocity * dt;
                }
            }
        }
    }
}