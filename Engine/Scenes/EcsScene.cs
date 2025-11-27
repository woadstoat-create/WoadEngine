using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.ECS;

namespace WoadEngine.Scenes
{
    public abstract class EcsScene : Scene
    {
        protected readonly List<Entity> entities = new();
        protected readonly List<IUpdateSystem> updateSystems = new ();
        protected readonly List<IDrawSystem> drawSystems = new();

        protected EcsScene(Game game) : base(game)
        {
            
        }

        protected virtual void ConfigureSystems() { }

        protected virtual void CreateInitialEntities() { }

        public override void Initialize()
        {
            base.Initialize();
            ConfigureSystems();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            CreateInitialEntities();
        }

        public override void Update(GameTime gt)
        {
            foreach (var system in updateSystems)
            {
                system.Update(gt, entities);
            }
        }

        public override void Draw(GameTime gt, SpriteBatch sb)
        {
            foreach (var system in drawSystems)
            {
                system.Draw(sb, entities);
            }       
        }
    }
}