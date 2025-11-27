using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.ECS
{
    public sealed class TextRenderSystem : IDrawSystem
    {
        public void Draw(SpriteBatch sb, IReadOnlyList<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (!e.TryGetComponent<TextComponent>(out var textComp) ||
                    !e.TryGetComponent<TransformComponent>(out var transform) ||
                    textComp.Font == null)
                    continue;

                sb.DrawString(
                    textComp.Font,
                    textComp.Text,
                    transform.Position,
                    textComp.Color,
                    transform.Rotation,
                    Vector2.Zero,
                    transform.Scale,
                    SpriteEffects.None,
                    1f
                );
            }
        }
    }
}