using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.UI.Controls;

public class Panel : UiContainer
{
    public Color? BackgroundColor { get; set; }
    public Color? BorderColor { get; set; }
    public int? BorderThickness { get; set; }

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return;

        var bg = BackgroundColor ?? Style?.BackgroundColor ?? Color.Transparent;
        var border = BorderColor ?? Style?.BorderColor ?? Color.Transparent;
        var borderThickness = BorderThickness ?? Style?.BorderThickness ?? 0;

        if (bg.A > 0)
            Core.SpriteBatch.Draw(whitePixel, Bounds, bg);

        if (borderThickness > 0 && border.A > 0)
            DrawBorder(whitePixel, Bounds, borderThickness, border);

        base.Draw(whitePixel);
    }

    public static void DrawBorder(Texture2D pixel, Rectangle rect, int thickness, Color color)
    {
        Core.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        Core.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        Core.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        Core.SpriteBatch.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
    }
}