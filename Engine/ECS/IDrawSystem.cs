using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.ECS
{
    public interface IDrawSystem
    {
        void Draw(SpriteBatch sb, IReadOnlyList<Entity> entities);
    }
}