using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Scenes
{
    public abstract class Scene
    {
        protected readonly Game Game;
        protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        protected ContentManager Content => Game.Content;


        protected Scene(Game game)
        {
            Game = game;
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gt) { }
        public virtual void Draw(GameTime gt, SpriteBatch sb) { }
    }
}