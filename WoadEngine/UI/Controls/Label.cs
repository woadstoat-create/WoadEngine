using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.UI.Controls;

public class Label : UiElement
{
    public string Text { get; set; } = string.Empty;
    public Color? TextColor { get; set; }

    public bool AutoSize { get; set; } = true;

    public override void Layout(Rectangle availableRect)
    {
        var font = Style?.Font;
        if (AutoSize && font != null)
        {
            var size = font.MeasureString(string.IsNullOrEmpty(Text) ? " " : Text);
            Width = (int)MathF.Ceiling(size.X);
            Height = (int)MathF.Ceiling(size.Y);
        }   

        base.Layout(availableRect);
    }

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return; 

        var font = Style?.Font;
        if (font != null && !string.IsNullOrEmpty(Text))
        {
            var color = Enabled 
                ? (TextColor ?? Style?.ForegroundColor ?? Color.White)
                : (Style?.DisabledForegroundColor ?? Color.Gray);

            Core.SpriteBatch.DrawString(font, Text, new Vector2(Bounds.X, Bounds.Y), color);
        }

        base.Draw(whitePixel);
    }
}