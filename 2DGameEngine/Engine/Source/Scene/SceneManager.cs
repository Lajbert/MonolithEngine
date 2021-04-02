using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.UI.Interface;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Source.Camera2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonolithEngine.Engine.Source.Scene
{
    public class SceneManager
    {
        private Dictionary<string, AbstractScene> scenes = new Dictionary<string, AbstractScene>();
        private HashSet<AbstractScene> activeScenes = new HashSet<AbstractScene>();
        private AbstractScene currentScene;
        private AbstractScene loadingScreen;
        private AbstractScene nextSceneToLoad;
        private AbstractScene nextSceneToStart;
        private Camera camera;

        private bool isLoading = false;

        private bool useLoadingScreen = false;

        public SceneManager(Camera camera)
        {
            this.camera = camera;
        }

        public void AddScene(AbstractScene scene)
        {
            if (scenes.ContainsKey(scene.GetName()))
            {
                throw new Exception("Scene name already exists!");
            }
            scenes.Add(scene.GetName(), scene);
            scene.Camera = camera;
            scene.SetSceneManager(this);
            if (scene.Preload)
            {
                scene.Load();
            }
        }

        public void SetLoadingScene(AbstractScene loadingScene)
        {
            this.loadingScreen = loadingScene;
        }

        public void OnResolutionChanged()
        {
            foreach (AbstractScene scene in scenes.Values)
            {
                scene.OnResolitionChanged();
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
            nextSceneToLoad = scenes[sceneName];
        }

        private void LoadNextScene()
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                data = currentScene.ExportData();
                currentScene.OnEnd();
                currentScene.Unload();
                if (!currentScene.AlwaysActive)
                {
                    activeScenes.RemoveIfExists(currentScene);
                }
            }
            currentScene = nextSceneToLoad;
            nextSceneToLoad = null;
            activeScenes.AddIfMissing(currentScene);
            currentScene.Load();
            currentScene.ImportData(data);
            currentScene.OnStart();
            isLoading = false;
            useLoadingScreen = false;
        }

        public void StartScene(string sceneName)
        {
            nextSceneToStart = scenes[sceneName];
        }

        private void StartNextScene()
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                data = currentScene.ExportData();
                currentScene.OnEnd();
                if (!currentScene.AlwaysActive)
                {
                    activeScenes.RemoveIfExists(currentScene);
                }
            }
            currentScene = nextSceneToStart;
            nextSceneToStart = null;
            activeScenes.AddIfMissing(currentScene);
            currentScene.ImportData(data);
            currentScene.OnStart();
            isLoading = false;
            useLoadingScreen = false;
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
            foreach (AbstractScene scene in activeScenes)
            {
                scene.FixedUpdate();
            }

            HandleSceneTransition();
        }

        internal void Update()
        {
            foreach (AbstractScene scene in activeScenes)
            {
                scene.Update();
            }

            HandleSceneTransition();
        }

        private void HandleSceneTransition()
        {
            if (nextSceneToLoad != null)
            {
                if (nextSceneToLoad.UseLoadingScreen && !isLoading && loadingScreen != null)
                {
                    useLoadingScreen = true;
                    return;
                }
                LoadNextScene();
            }
            if (nextSceneToStart != null)
            {
                if (nextSceneToStart.UseLoadingScreen && !isLoading && loadingScreen != null)
                {
                    useLoadingScreen = true;
                    return;
                }
                StartNextScene();
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (useLoadingScreen && loadingScreen != null)
            {
                isLoading = true;
                loadingScreen.Draw(spriteBatch);
            } else
            {
                currentScene.Draw(spriteBatch);
            }
        }

    }
}
