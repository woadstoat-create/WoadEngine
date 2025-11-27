using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WoadEngine.ECS
{
    public sealed class PlayerInputSystem : IUpdateSystem
    {
        public void Update(GameTime gt, IReadOnlyList<Entity> entities)
        {
            var kb = Keyboard.GetState();

            foreach (var e in entities)
            {
                if (!e.TryGetComponent<InputComponent>(out var input) ||
                    !e.TryGetComponent<VelocityComponent>(out var velocity) ||
                    !e.TryGetComponent<TransformComponent>(out var transform))
                    continue;

                Vector2 dir = Vector2.Zero;

                if (kb.IsKeyDown(input.LeftKey))  dir.X -= 1f;
                if (kb.IsKeyDown(input.RightKey)) dir.X += 1f;
                if (kb.IsKeyDown(input.UpKey))    dir.Y -= 1f;
                if (kb.IsKeyDown(input.DownKey))  dir.Y += 1f;

                if (input.VerticalOnly)
                    dir.X = 0f;
                if (input.HorizontalOnly)
                    dir.Y = 0f;

                if (dir != Vector2.Zero)
                    dir.Normalize();

                velocity.Velocity = dir * input.MoveSpeed;
            }
        }
    }
}