using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WoadEngine.UI.Controls;

public class Slider : UiElement
{
    private bool _isDragging;

    public float Min { get; set; } = 0f;
    public float Max { get; set; } = 100f;

    private float _value; 
    public float Value 
    {
        get => _value;
        set 
        {
            float clamped = MathHelper.Clamp(value, Min, Max);
            if (Math.Abs(clamped - _value) < 0.0001f)
                return;

            _value = clamped;
            ValueChanged?.Invoke(this, _value);
        }
    }

    public int TrackHeight { get; set; } = 6;
    public int ThumbWidth { get; set; } = 14;
    public int ThumbHeight { get; set; } = 24;

    public Color TrackColor { get; set; } = new(50, 50, 50);
    public Color FillColor { get; set; } = Color.LimeGreen;
    public Color ThumbColor { get; set; } = Color.White;
    public Color HoverThumbColor { get; set; } = new(220, 220, 220);
    public Color PressedThumbColor { get; set; } = Color.Yellow;
    public Color BorderColor { get; set; } = Color.Black;
    public int BorderThickness { get; set; } = 1;

    public bool ClickToJump { get; set; } = true;

    public event Action<Slider, float>? ValueChanged;

    public Slider()
    {
        AcceptsMouseInput = true;
        CanFocus = true;
        Width = 200;
        Height = 24;
    }

    public float NormalizedValue
    {
        get 
        {
            float range = Max - Min;
            if (range <= 0f)
                return 0f;
            
            return (Value - Min) / range;
        }
    }

    public Rectangle TrackBounds
    {
        get
        {
            int y = Bounds.Y + ((Bounds.Height - TrackHeight) / 2);
            return new Rectangle(Bounds.X, y, Bounds.Width, TrackHeight);
        }
    }

    public Rectangle ThumbBounds
    {
        get
        {
            var track = TrackBounds;

            int usableWidth = Math.Max(1, track.Width - ThumbWidth);
            int thumbX = track.X + (int)(usableWidth * NormalizedValue);
            int thumbY = Bounds.Y + ((Bounds.Height - ThumbHeight) / 2);

            return new Rectangle(thumbX, thumbY, ThumbWidth, ThumbHeight);
        }
    }

    public override UiElement? HitTest(Point point)
    {
        if (!Visible || !Enabled)
            return null;

        if (Bounds.Contains(point))
            return this;

        return null;
    }

    public override void OnMouseDown()
    {
        _isDragging = true;
    }

    public override void OnMouseUp()
    {
        _isDragging = false;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public void UpdateInteraction(Point mousePosition, bool isMouseDown)
    {
        if (!Enabled || !Visible)
            return;

        if (_isDragging && isMouseDown)
        {
            SetValueFromMouse(mousePosition.X);
            return;
        }

        if (!isMouseDown)
            _isDragging = false;
    }

    public override void Draw(Texture2D whitePixel)
    {
        if (!Visible)
            return;

        var track = TrackBounds;
        var thumb = ThumbBounds;

        Core.SpriteBatch.Draw(whitePixel, track, TrackColor);

        int fillWidth = thumb.Center.X - track.X;
        if (fillWidth > 0)
        {
            var fillRect = new Rectangle(track.X, track.Y, fillWidth, track.Height);
            Core.SpriteBatch.Draw(whitePixel, fillRect, FillColor);
        }

        Color thumbColor = ThumbColor;
        if (IsPressed || _isDragging)
            thumbColor = PressedThumbColor;
        else if (IsHovered)
            thumbColor = HoverThumbColor;

        Core.SpriteBatch.Draw(whitePixel, thumb, thumbColor);

        if (BorderThickness > 0)
        {
            Panel.DrawBorder(whitePixel, track, BorderThickness, BorderColor);
            Panel.DrawBorder(whitePixel, thumb, BorderThickness, BorderColor);
        }

        base.Draw(whitePixel);
    }

    public void JumpToMouse(Point mousePosition)
    {
        if (!ClickToJump)
            return;

        SetValueFromMouse(mousePosition.X);
    }

    private void SetValueFromMouse(int mouseX)
    {
        var track = TrackBounds;

        float localX = mouseX - track.X - (ThumbWidth * 0.5f);
        float usableWidth = Math.Max(1, track.Width - ThumbWidth);
        float t = MathHelper.Clamp(localX / usableWidth, 0f, 1f);

        Value = MathHelper.Lerp(Min, Max, t);
    }
}