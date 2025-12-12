using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.ECS
{
    public sealed class SpriteComponent : IComponent
    {
        public required Texture2D Texture;
        public Color Color = Color.White;
        public Vector2 Origin;
        public float Alpha = 1.0f;
    }
}