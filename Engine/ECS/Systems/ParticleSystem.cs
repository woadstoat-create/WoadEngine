using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class ParticleSystem : IUpdateSystem
    {
        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var e in entities)
            {
                if (!e.TryGetComponent<ParticleComponent>(out var particle))
                    continue;

                particle.Age += dt;

                if (particle.Age >= particle.LifeTime)
                {
                    if(particle.DestroyOnEnd)
                    {
                        e.RemoveComponent<SpriteComponent>();
                        e.RemoveComponent<VelocityComponent>();
                        e.RemoveComponent<ParticleComponent>();
                    }

                    continue;
                }

                float t = particle.LifeTime > 0 ? particle.Age / particle.LifeTime : 1f;

                if (particle.UseColorLerp && 
                    e.TryGetComponent<SpriteComponent>(out var sprite))
                {
                    sprite.Color = Color.Lerp(
                        particle.StartColor,
                        particle.EndColor,
                        MathHelper.Clamp(t, 0f, 1f)
                    );
                }

                if (particle.UseScaleLerp &&
                    e.TryGetComponent<TransformComponent>(out var transform))
                {
                    transform.Scale = Vector2.Lerp(
                        particle.StartScale,
                        particle.EndScale,
                        MathHelper.Clamp(t, 0f, 1f));
                }
            }
        }
    }
}