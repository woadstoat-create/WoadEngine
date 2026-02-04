using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Rendering;

public class TextureRegion
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public int Width => SourceRectangle.Width;
    public int Height => SourceRectangle.Height;

    public TextureRegion() { }

    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    public void Draw(SpriteBatch sb, Vector2 pos, Color col)
    {
        Draw(sb, pos, col, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
    }

    public void Draw(SpriteBatch sb, Vector2 pos, Color col, float rot, Vector2 origin, float scale, SpriteEffects effects, float layer)
    {
        Draw(sb, pos, col, rot, origin, new Vector2(scale, scale), effects, layer);
    }

    public void Draw(SpriteBatch sb, Vector2 pos, Color col, float rot, Vector2 origin, Vector2 scale, SpriteEffects effects, float layer)
    {
        sb.Draw(
            Texture,
            pos,
            SourceRectangle,
            col,
            rot,
            origin,
            scale,
            effects,
            layer
        );
    }
}