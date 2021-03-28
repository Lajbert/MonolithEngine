using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Scene
{
    public class SceneManager
    {
        private Dictionary<string, AbstractScene> scenes = new Dictionary<string, AbstractScene>();
        private AbstractScene currentScene;

        public void AddScene(AbstractScene scene)
        {
            if (scenes.ContainsKey(scene.GetName()))
            {
                throw new Exception("Scene name already exists!");
            }
            scenes.Add(scene.GetName(), scene);
            scene.SetSceneManager(this);
            if (scene.Preload)
            {
                scene.Load();
            }
        }

        internal void OnSceneFinished(IScene scene)
        {

        }

        public void RemoveScene(AbstractScene scene)
        {
            scenes.Remove(scene.GetName());
        }

        public void LoadScene(string sceneName)
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                data = currentScene.ExportData();
                currentScene.OnEnd();
                currentScene.Unload();
            }
            currentScene = scenes[sceneName];
            currentScene.Load();
            currentScene.ImportData(data);
            currentScene.OnStart();
        }

        public void StartScene(string sceneName)
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                data = currentScene.ExportData();
            }
            currentScene = scenes[sceneName];
            currentScene.ImportData(data);
            currentScene.OnStart();
        }

        public void StartScene(AbstractScene scene)
        {
            StartScene(scene.GetName());
        }

        public void LoadScene(AbstractScene scene)
        {
            LoadScene(scene.GetName());
        }

        public void FixedUpdate()
        {
            currentScene.FixedUpdate();
        }

        internal void Update()
        {
            currentScene.Update();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            currentScene.Draw(spriteBatch);
        }

    }
}
