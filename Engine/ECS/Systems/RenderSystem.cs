using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.ECS
{
    public sealed class RenderSystem : IDrawSystem
    {
        public void Draw(SpriteBatch sb, IReadOnlyList<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e.TryGetComponent<TransformComponent>(out var transform) &&
                    e.TryGetComponent<SpriteComponent>(out var sprite) &&
                    sprite.Texture != null)
                {
                    sb.Draw(
                        sprite.Texture,
                        transform.Position,
                        null,
                        sprite.Color * sprite.Alpha,
                        transform.Rotation,
                        sprite.Origin,
                        transform.Scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}