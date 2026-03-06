using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WoadEngine.Scenes;

namespace WoadEngine.UI;

public sealed class UiManager
{
    private readonly Texture2D _whitePixel;

    public UiElement? GlobalRoot { get; set; }
    public UiElement? SceneRoot { get; set; }

    public UiElement? HoveredElement { get; set; }
    public UiElement? PressedElement { get; set; }
    public UiElement? FocusedElement { get; set; }

    public UiTheme Theme { get; } = new();

    public bool PointerConsumedThisFrame { get; private set; }


    public UiManager()
    {
        _whitePixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        _whitePixel.SetData(new[] { Color.White });
    }

    public void Update(float dt, Rectangle viewportRect, UiMouseState mouse)
    {
        PointerConsumedThisFrame = false;

        GlobalRoot?.Layout(viewportRect);
        SceneRoot?.Layout(viewportRect);

        GlobalRoot?.Update(dt);
        SceneRoot?.Update(dt);

        var hovered = HitTest(mouse.Position);

        if (hovered != HoveredElement)
        {
            HoveredElement?.OnMouseLeave();
            HoveredElement = hovered;
            HoveredElement?.OnMouseEnter();
        }

        if (HoveredElement != null)
            PointerConsumedThisFrame = true;

        if (mouse.LeftPressed)
        {
            PressedElement = HoveredElement;
            if (PressedElement != null)
            {
                PressedElement.IsPressed = true;
                PressedElement.OnMouseDown();
                PointerConsumedThisFrame = true;
            }
        }

        if (mouse.LeftReleased)
        {
            if (PressedElement != null)
            {
                PressedElement.OnMouseUp();

                bool clickedSameElement = HoveredElement == PressedElement;
                PressedElement.IsPressed = false;

                if (clickedSameElement)
                {
                    SetFocus(PressedElement);
                    PressedElement.OnClick();
                    PointerConsumedThisFrame = true;
                }

                PressedElement = null;
            }
        }

        UpdateHoverFlags(GlobalRoot, hovered);
        UpdateHoverFlags(SceneRoot, hovered);
    }

    public void Draw()
    {
        Core.SpriteBatch.Begin();
        GlobalRoot?.Draw(_whitePixel);
        SceneRoot?.Draw(_whitePixel);
        Core.SpriteBatch.End();
    }

    public UiElement? HitTest(Point point)
    {
        var globalHit = GlobalRoot?.HitTest(point);
        if (globalHit != null)
            return globalHit;

        return SceneRoot?.HitTest(point);
    }

    public void SetFocus(UiElement? element)
    {
        if (FocusedElement == element)
            return;

        if (FocusedElement != null)
            FocusedElement.IsFocused = false;

        FocusedElement = element;

        if (FocusedElement != null)
            FocusedElement.IsFocused = true;
    }

    private static void UpdateHoverFlags(UiElement? root, UiElement? hovered)
    {
        if (root == null)
            return;

        root.IsHovered = ReferenceEquals(root, hovered);

        foreach (var child in root.Children)
            UpdateHoverFlags(child, hovered);
    }
}