using Microsoft.Xna.Framework;

namespace WoadEngine.UI;

public readonly struct UiMouseState
{
    public readonly Point Position;
    public readonly bool LeftDown;
    public readonly bool LeftPressed;
    public readonly bool LeftReleased;

    public UiMouseState(Point position, bool leftDown, bool leftPressed, bool leftReleased)
    {
        Position = position;
        LeftDown = leftDown;
        LeftPressed = leftPressed;
        LeftReleased = leftReleased;
    }
}