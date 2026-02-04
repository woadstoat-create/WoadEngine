using Microsoft.Xna.Framework;

namespace WoadEngine.Physics;

public class Collider
{
    private Rectangle _rect;
    private Vector2 _offset;

    public Rectangle Rect => _rect;

    public Collider(int x, int y, int width, int height, Vector2 offset)
    {
        _rect = new Rectangle(x, y, width, height);
        _offset = offset;
    }

    public void UpdatePosition(Vector2 pos)
    {
        _rect.X = (int)(pos.X - _offset.X);
        _rect.Y = (int)(pos.Y - _offset.Y);
    }

    public bool GetCollision(Collider other)
    {
        return _rect.Intersects(other.Rect);
    }
}