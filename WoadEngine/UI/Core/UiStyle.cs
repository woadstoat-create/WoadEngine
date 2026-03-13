using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.UI;

public sealed class UiStyle
{
    public SpriteFont? Font { get; set; }

    public Color TextColor { get; set; } = Color.White;

    public Color ForegroundColor { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public Color BorderColor { get; set; } = Color.Transparent;

    public Color HoverBackgroundColor { get; set; } = Color.Transparent;
    public Color PressedBackgroundColor { get; set; } = Color.Transparent;
    public Color DisabledForegroundColor { get; set; } = Color.Gray;
    public Color DisabledBackgroundColor { get; set; } = new Color(40, 40, 40);

    public int BorderThickness { get; set; } = 0;
    public Thickness Padding { get; set; } = Thickness.Zero;
}