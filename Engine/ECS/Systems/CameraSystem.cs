using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class CameraSystem : IUpdateSystem
    {
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private readonly Random _rng = new Random();

        public CameraSystem(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            CameraComponent? camera = null;
            CameraShakeComponent? shake = null;

            // Find the first entity with a camera
            foreach (var e in entities)
            {
                if (e.TryGetComponent<CameraComponent>(out var cam))
                {
                    camera = cam;
                    e.TryGetComponent<CameraShakeComponent>(out shake);
                    break;
                }
            }

            if (camera == null)
            {
                CameraState.Transform = Matrix.Identity;
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Compute shake offset
            Vector2 shakeOffset = Vector2.Zero;

            if (shake != null && shake.TimeRemaining > 0f && shake.InitialDuration > 0f && shake.Amplitude > 0f)
            {
                shake.TimeRemaining -= dt;
                if (shake.TimeRemaining < 0f)
                    shake.TimeRemaining = 0f;

                float t = shake.TimeRemaining / shake.InitialDuration; // 1 -> 0
                t = MathHelper.Clamp(t, 0f, 1f);

                float currentAmplitude = shake.Amplitude * t;

                float x = ((float)_rng.NextDouble() * 2f - 1f) * currentAmplitude;
                float y = ((float)_rng.NextDouble() * 2f - 1f) * currentAmplitude;

                shakeOffset = new Vector2(x, y);
            }

            // For now: base 2D camera = just translation by Offset + shake
            var totalOffset = camera.Offset + shakeOffset;

            // Positive offset means move the world by that amount
            CameraState.Transform = Matrix.CreateTranslation(totalOffset.X, totalOffset.Y, 0f);
        }
    }
}