using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using WoadEngine.Diagnostics;

namespace WoadEngine.UI;

public abstract class UiElement
{
    private readonly List<UiElement> _children = new();

    public string? Name { get; set; }

    public UiElement? Parent { get; private set; }
    public IReadOnlyList<UiElement> Children => _children;

    public bool Visible { get; set; } = true;
    public bool Enabled { get; set; } = true;
    public bool CanFocus { get; set; } = false;
    public bool AcceptsMouseInput { get; set; } = false;

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public int MinWidth { get; set; }
    public int MinHeight { get; set; }

    public Thickness Margin { get; set; } = Thickness.Zero;
    public Thickness Padding { get; set; } = Thickness.Zero;
    public UiAnchor Anchor { get; set; } = UiAnchor.TopLeft;

    public Rectangle Bounds { get; protected set; }
    public Rectangle ContentBounds { get; protected set; }

    public UiStyle? Style { get; set; }

    public bool IsHovered { get; internal set; }
    public bool IsPressed { get; internal set; }
    public bool IsFocused { get; internal set; }

    public virtual void AddChild(UiElement child)
    {
        if (child.Parent != null)
            Logger.Exception(new InvalidOperationException("UI child already has a parent."));

        child.Parent = this;
        _children.Add(this);
    }

    public virtual void RemoveChild(UiElement child)
    {
        if (_children.Remove(child))
            child.Parent = null;
    }

    public virtual void ClearChildren()
    {
        foreach (var child in _children)
            child.Parent = null;

        _children.Clear();
    }

    public virtual void Update(float dt)
    {
        if (!Visible)
            return;
        
        foreach (var child in _children)
            child.Update(dt);
    }

    public virtual void Layout(Rectangle availableRect)
    {
        Bounds = ComputeAnchoredBounds(availableRect);

        ContentBounds = new Rectangle(
            Bounds.X + Padding.Left,
            Bounds.Y + Padding.Top,
            Math.Max(0, Bounds.Width - Padding.Horizontal),
            Math.Max(0, Bounds.Height - Padding.Vertical));

        foreach (var child in _children)
            child.Layout(ContentBounds);
    }

    protected virtual Rectangle ComputeAnchoredBounds(Rectangle parentRect)
    {
        int width = Math.Max(MinWidth, Width);
        int height = Math.Max(MinHeight, Height);

        int x = parentRect.X + X + Margin.Left;
        int y = parentRect.Y + Y + Margin.Top;

        switch (Anchor)
        {
            case UiAnchor.TopLeft:
                break;

            case UiAnchor.TopCenter:
                x = parentRect.Center.X - (width / 2) + X;
                y = parentRect.Y + Y + Margin.Top;
                break;

            case UiAnchor.TopRight:
                x = parentRect.Right - width - X - Margin.Right;
                y = parentRect.Y + Y + Margin.Top;
                break;

            case UiAnchor.CenterLeft:
                x = parentRect.X + X + Margin.Left;
                y = parentRect.Center.Y - (height / 2) + Y;
                break;

            case UiAnchor.Center:
                x = parentRect.Center.X - (width / 2) + X;
                y = parentRect.Center.Y - (height / 2) + Y;
                break;

            case UiAnchor.CenterRight:
                x = parentRect.Right - width - X - Margin.Right;
                y = parentRect.Center.Y - (height / 2) + Y;
                break;

            case UiAnchor.BottomLeft:
                x = parentRect.X + X + Margin.Left;
                y = parentRect.Bottom - height - Y - Margin.Bottom;
                break;

            case UiAnchor.BottomCenter:
                x = parentRect.Center.X - (width / 2) + X;
                y = parentRect.Bottom - height - Y - Margin.Bottom;
                break;

            case UiAnchor.BottomRight:
                x = parentRect.Right - width - X - Margin.Right;
                y = parentRect.Bottom - height - Y - Margin.Bottom;
                break;

            case UiAnchor.StretchHorizontal:
                x = parentRect.X + X + Margin.Left;
                y = parentRect.Y + Y + Margin.Top;
                width = Math.Max(MinWidth, parentRect.Width - X - Margin.Left - Margin.Right);
                break;

            case UiAnchor.StretchVertical:
                x = parentRect.X + X + Margin.Left;
                y = parentRect.Y + Y + Margin.Top;
                height = Math.Max(MinHeight, parentRect.Height - Y - Margin.Top - Margin.Bottom);
                break;

            case UiAnchor.StretchAll:
                x = parentRect.X + X + Margin.Left;
                y = parentRect.Y + Y + Margin.Top;
                width = Math.Max(MinWidth, parentRect.Width - X - Margin.Left - Margin.Right);
                height = Math.Max(MinHeight, parentRect.Height - Y - Margin.Top - Margin.Bottom);
                break;
        }

        return new Rectangle(x, y, width, height);
    }

    public virtual UiElement? HitTest(Point point)
    {
        if (!Visible || !Enabled)
            return null;

        for (int i = _children.Count - 1; i >= 0; i--)
        {
            var hit = _children[i].HitTest(point);
            if (hit != null)
                return hit;
        }

        if (AcceptsMouseInput && Bounds.Contains(point))
            return this;

        return null;
    }

    public virtual void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return;

        foreach (var child in _children)
            child.Draw(whitePixel);
    }

    public virtual void OnMouseEnter() { }
    public virtual void OnMouseLeave() { }
    public virtual void OnMouseDown() { }
    public virtual void OnMouseUp() { }
    public virtual void OnClick() { }
}