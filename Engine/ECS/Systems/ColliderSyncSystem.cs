using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class ColliderSyncSystem : IUpdateSystem
    {
        public void Update(GameTime gt, IReadOnlyList<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (!e.TryGetComponent<TransformComponent>(out var transform) ||
                    !e.TryGetComponent<ColliderComponent>(out var collider))
                    continue;

                int w = collider.Collider.Width;
                int h = collider.Collider.Height;

                int x = (int)(transform.Position.X - w / 2f);
                int y = (int)(transform.Position.Y - h / 2f);

                collider.Collider = new Rectangle(x, y, w, h);
            }
        }
    }
}