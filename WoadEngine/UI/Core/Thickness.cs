namespace WoadEngine.UI;

public readonly struct Thickness
{
    public readonly int Left;
    public readonly int Top;
    public readonly int Right;
    public readonly int Bottom;

    public Thickness(int uniform)
        : this(uniform, uniform, uniform, uniform) { }
    
    public Thickness(int horizontal, int vertical) 
        : this(horizontal, vertical, horizontal, vertical) { }

    public Thickness(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Horizontal => Left + Right;
    public int Vertical => Top + Bottom;

    public static readonly Thickness Zero = new(0);
}