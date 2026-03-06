using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WoadEngine.UI.Controls;

public class Button : Panel
{
    public string Text { get; set; } = string.Empty;
    public Color? TextColor { get; set; }

    public event Action<Button>? Clicked;

    public Button()
    {
        AcceptsMouseInput = true;
        CanFocus = true;
    }

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return;

        var bg = ResolveBackgroundColor();
        var border = BorderColor ?? Style?.BorderColor ?? Color.Transparent;
        var borderThickness = BorderThickness ?? Style?.BorderThickness ?? 0;

        if (bg.A > 0)
            Core.SpriteBatch.Draw(whitePixel, Bounds, bg);

        if (borderThickness > 0 && border.A > 0)
            DrawBorder(whitePixel, Bounds, borderThickness, border);

        var font = Style?.Font;
        if (font != null && !string.IsNullOrEmpty(Text))
        {
            var measure = font.MeasureString(Text);
            var pos = new Vector2(
                Bounds.X + ((Bounds.Width - measure.X) / 2f),
                Bounds.Y + ((Bounds.Height - measure.Y) / 2f));

            var color = Enabled
                ? (TextColor ?? Style?.ForegroundColor ?? Color.White)
                : (Style?.DisabledForegroundColor ?? Color.Gray);

            Core.SpriteBatch.DrawString(font, Text, pos, color);
        }

        base.Draw(whitePixel);
    }

    private Color ResolveBackgroundColor()
    {
        if (!Enabled)
            return Style?.DisabledBackgroundColor ?? Color.DarkGray;

        if (IsPressed)
            return Style?.PressedBackgroundColor
                   ?? Style?.HoverBackgroundColor
                   ?? Style?.BackgroundColor
                   ?? Color.Gray;

        if (IsHovered)
            return Style?.HoverBackgroundColor
                   ?? Style?.BackgroundColor
                   ?? Color.Gray;

        return BackgroundColor
               ?? Style?.BackgroundColor
               ?? Color.Gray;
    }

    public override void OnClick()
    {
        Clicked?.Invoke(this);
    }

}