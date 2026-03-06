using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.UI.Controls;

public class Label : UiElement
{
    public string Text { get; set; } = string.Empty;
    public Color? TextColor { get; set; }

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return; 

        var font = Style?.Font;
        if (font != null && !string.IsNullOrEmpty(Text))
        {
            var color = Enabled 
                ? (TextColor ?? Style?.ForegroundColor ?? Color.White)
                : (Style?.DisabledBackgroundColor ?? Color.Gray);

            Core.SpriteBatch.DrawString(font, Text, new Vector2(Bounds.X, Bounds.Y), color);
        }

        base.Draw(whitePixel);
    }
}