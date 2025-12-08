using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WoadEngine.Services;

namespace WoadEngine.ECS
{
    /// <summary>
    /// Shows queued notifications as small toasts (e.g., top-right).
    /// Uses TextComponent + TransformComponent + ParticleComponent.
    /// </summary>
    public sealed class NotificationPopupSystem : IUpdateSystem
    {
        private readonly SpriteFont _font;
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private Entity _currentToastEntity;
        private float _timeRemaining;

        public NotificationPopupSystem(SpriteFont font, int viewportWidth, int viewportHeight)
        {
            _font = font;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            var list = entities as IList<Entity>;
            if (list == null)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // If we have a toast currently active, update timer & cleanup when done
            if (_currentToastEntity != null)
            {
                _timeRemaining -= dt;
                if (_timeRemaining <= 0f)
                {
                    _currentToastEntity.RemoveComponent<TextComponent>();
                    _currentToastEntity.RemoveComponent<TransformComponent>();
                    _currentToastEntity.RemoveComponent<ParticleComponent>();
                    list.Remove(_currentToastEntity);
                    _currentToastEntity = null;
                }

                return;
            }

            // No active toast → check queue
            if (!NotificationService.HasPending)
                return;

            if (!NotificationService.TryDequeue(out var req))
                return;

            // Build visible text (you can style this however you like)
            string text = string.IsNullOrEmpty(req.Message)
                ? req.Title
                : $"{req.Title}\n{req.Message}";

            // Spawn toast entity
            var e = new Entity();

            // Top-right anchor
            var padding = new Vector2(16f, 16f);

            e.AddComponent(new TransformComponent
            {
                Position = new Vector2(
                    _viewportWidth - padding.X,
                    padding.Y),
                Scale = Vector2.One
            });

            e.AddComponent(new TextComponent
            {
                Font = _font,
                Text = text,
                Color = KindToColor(req.Kind),
                
            });

            e.AddComponent(new ParticleComponent
            {
                LifeTime = req.DurationSeconds,
                DestroyOnEnd = false,     // we remove in here
                UseColorLerp = true,
                StartColor = KindToColor(req.Kind),
                EndColor = Color.Transparent,
                UseScaleLerp = false
            });

            list.Add(e);
            _currentToastEntity = e;
            _timeRemaining = req.DurationSeconds;
        }

        private Color KindToColor(NotificationKind kind)
        {
            return kind switch
            {
                NotificationKind.Success     => Color.LimeGreen,
                NotificationKind.Warning     => Color.Orange,
                NotificationKind.Achievement => Color.Gold,
                _                            => Color.White,
            };
        }
    }
}