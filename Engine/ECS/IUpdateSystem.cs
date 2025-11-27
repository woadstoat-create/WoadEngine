using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public interface IUpdateSystem
    {
        void Update(GameTime dt, IReadOnlyList<Entity> entities);
    }
}