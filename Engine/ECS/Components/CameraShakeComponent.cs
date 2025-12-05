using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class CameraShakeComponent : IComponent
    {
        public float TimeRemaining = 0f;
        public float InitialDuration = 0f;
        public float Amplitude = 0f;
    }
}