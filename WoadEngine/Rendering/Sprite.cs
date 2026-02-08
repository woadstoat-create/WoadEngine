using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Rendering;

public class Sprite
{
    private TextureRegion _region;
    private Color _color = Color.White;
    private float _rotation = 0, _layer = 1;
    private Vector2 _origin = Vector2.Zero, _scale = new Vector2(1, 1);
    private SpriteEffects _effect = SpriteEffects.None;
    
    public float Width => _region.Width * _scale.X;
    public float Height => _region.Height * _scale.Y;

    public float Rotation
    {
        get { return _rotation; }
        set { _rotation = value; }
    }

    public Vector2 Scale
    {
        get { return _scale; }
        set { _scale = value; }
    }

    public Sprite(TextureRegion region)
    {
        _region = region;
    }

    public void CenterOrigin()
    {
        _origin = new Vector2(_region.Width, _region.Height) * 0.5f;
    }

    public void Draw(SpriteBatch sb, Vector2 pos)
    {
        _region.Draw(sb, pos, _color, _rotation, _origin, _scale, _effect, _layer);
    }
}