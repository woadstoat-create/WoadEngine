using Microsoft.Xna.Framework;

namespace WoadEngine.Physics;

public class Transform
{
    private Vector2 _position; 

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public float Y
    {
        get { return _position.Y; }
        set { _position.Y = value; }
    }

    public float X
    {
        get { return _position.X; }
        set { _position.X = value; }
    }

    public Transform()
    {
        _position = new Vector2();
    }

    public Transform(Vector2 pos)
    {
        _position = pos;
    }

    public void Move(Vector2 pos, GameTime gt)
    {
        _position += pos * (float)gt.ElapsedGameTime.TotalSeconds;
    }

    public void Move(int x, int y, GameTime gt)
    {
        Move(new Vector2(x, y), gt);
    }
}
