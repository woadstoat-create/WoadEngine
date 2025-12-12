using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Scenes
{
    public enum SceneId
    {
        Splash,
        MainMenu,
        Play,
        Pause, 
        LevelSelect,
        Options,
        Survival,
        Credits
    }

    public class SceneManager
    {
        private readonly Dictionary<SceneId, Scene> _scenes = new();
        private required Scene _currentScene;
        private SceneId? _currentId;

        public void AddScene(SceneId id, Scene scene)
        {
            _scenes[id] = scene;
        }

        public void ChangeScene(SceneId id)
        {
            if (_currentScene != null)
            {
                _currentScene?.UnloadContent();
            }

            if (_scenes.TryGetValue(id, out var newScene))
            {
                _currentScene = newScene;
                _currentId = id;

                _currentScene.Initialize();
                _currentScene.LoadContent();
            }
        }

        public void Update(GameTime gt)
        {
            _currentScene?.Update(gt);
        }

        public void Draw(GameTime gt, SpriteBatch sb)
        {
            _currentScene?.Draw(gt, sb);
        }
    }
}