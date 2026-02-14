#if DEBUG
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WoadEngine;
using WoadEngine.ECS;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Systems;

public sealed class DebugColliderRenderSystem : IRenderSystem
{
    private readonly Texture2D _pixel;
    private bool _enabled = true;

    public DebugColliderRenderSystem()
    {
        _pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(World world, float dt)
    {
        if (Core.Input.Keyboard.WasKeyPressed(Keys.F2))
            _enabled = !_enabled;
            
        if (_enabled)
        {
            var transforms = world.GetStore<Transform>();
            var colliders = world.GetStore<Collider2D>();

            var ents = colliders.DenseEntities;

            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!transforms.Has(id)) continue;

                ref var c = ref colliders.DenseComponents[i];
                ref var t = ref transforms.Get(id);

                Rectangle rect = ComputeWorldRect(t, c);

                DrawRectangle(Core.SpriteBatch, rect, Color.Lime);
            }
        }
    }

    private static Rectangle ComputeWorldRect(in Transform t, in Collider2D c)
    {
        int x = (int)(t.Position.X + c.Offset.X + c.Rect.X);
        int y = (int)(t.Position.Y + c.Offset.Y + c.Rect.Y);

        return new Rectangle(x, y, c.Rect.Width, c.Rect.Height);
    }

    private void DrawRectangle(SpriteBatch sb, Rectangle rect, Color color)
    {
        const int thickness = 1;

        // Top
        sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        sb.Draw(_pixel, new Rectangle(rect.X, rect.Bottom, rect.Width, thickness), color);
        // Left
        sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        sb.Draw(_pixel, new Rectangle(rect.Right, rect.Y, thickness, rect.Height), color);
    }
}
#endif