using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.UI.Controls;

public class ProgressBar : UiElement
{
    public float Value { get; set; }
    public float Max { get; set; } = 100f;

    public Color FillColor { get; set; } = Color.LimeGreen;
    public Color BackgroundColor { get; set; } = new(25, 25, 25);
    public Color BorderColor { get; set; } = Color.White;
    public int BorderThickness { get; set; } = 1;

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return;

        Core.SpriteBatch.Draw(whitePixel, Bounds, BackgroundColor);

        float ratio = Max <= 0f ? 0f : MathHelper.Clamp(Value / Max, 0f, 1f);
        int fillWidth = (int)(Bounds.Width * ratio);

        if (fillWidth > 0)
        {
            var fillRect = new Rectangle(Bounds.X, Bounds.Y, fillWidth, Bounds.Height);
            Core.SpriteBatch.Draw(whitePixel, fillRect, FillColor);
        }

        if (BorderThickness > 0)
            Panel.DrawBorder(whitePixel, Bounds, BorderThickness, BorderColor);

        base.Draw(whitePixel);
    }
}