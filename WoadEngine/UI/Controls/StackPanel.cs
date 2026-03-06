using System;
using Microsoft.Xna.Framework;

namespace WoadEngine.UI.Controls;

public enum StackOrientation
{
    Vertical,
    Horizontal
}

public class StackPanel : UiContainer
{
    public StackOrientation Orientation { get; set; } = StackOrientation.Vertical;
    public int Spacing { get; set; } = 0;

    public override void Layout(Rectangle availableRect)
    {
        Bounds = ComputeAnchoredBounds(availableRect);

        ContentBounds = new Rectangle(
            Bounds.X + Padding.Left,
            Bounds.Y + Padding.Top,
            Math.Max(0, Bounds.Width - Padding.Horizontal),
            Math.Max(0, Bounds.Height - Padding.Vertical));

        int offset = 0;

        foreach (var child in Children)
        {
            if (!child.Visible)
                continue;

            if (Orientation == StackOrientation.Vertical)
            {
                child.X = 0;
                child.Y = offset;
                child.Anchor = UiAnchor.TopLeft;
                child.Layout(ContentBounds);

                offset += child.Bounds.Height + Spacing;
            }
            else
            {
                child.X = offset;
                child.Y = 0;
                child.Anchor = UiAnchor.TopLeft;
                child.Layout(ContentBounds);

                offset += child.Bounds.Width + Spacing;
            }
        }
    }
}