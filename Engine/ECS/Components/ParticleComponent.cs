using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class ParticleComponent : IComponent
    {
        public float LifeTime = 1f;
        public float Age = 0f;
        public bool DestroyOnEnd = true;
        public bool UseColorLerp = true;
        public Color StartColor = Color.White;
        public Color EndColor = Color.Transparent;
        public bool UseScaleLerp = false;
        public Vector2 StartScale = Vector2.One;
        public Vector2 EndScale = Vector2.One;
    }
}