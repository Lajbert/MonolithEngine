using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Class to handle scenes.
    /// </summary>
    public class SceneManager
    {
        // all the scenes the game has
        private Dictionary<string, AbstractScene> scenes = new Dictionary<string, AbstractScene>();

        // all the scenes that are being updated
        private HashSet<AbstractScene> activeScenes = new HashSet<AbstractScene>();

        // the scene that is currently running and visible on the display
        private AbstractScene currentScene;

        // static loading screen cene
        private AbstractScene loadingScreen;

        private AbstractScene nextSceneToLoad;
        private AbstractScene nextSceneToStart;

        private Camera camera;

        private GraphicsDevice graphicsDevice;

        private bool isLoading = false;

        private bool useLoadingScreen = false;

        public SceneManager(Camera camera, GraphicsDevice graphicsDevice)
        {
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Adds a new scene to the game.
        /// </summary>
        /// <param name="scene"></param>
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
                scene.InternalLoad();
            }
        }

        /// <summary>
        /// The loading scene to use for where 'useLoadingScreen' is true.
        /// </summary>
        /// <param name="loadingScene"></param>
        public void SetLoadingScene(AbstractScene loadingScene)
        {
            this.loadingScreen = loadingScene;
        }

        /// <summary>
        /// Called when the user changes resolutions.
        /// </summary>
        public void OnResolutionChanged()
        {
            foreach (AbstractScene scene in scenes.Values)
            {
                scene.OnResolitionChanged();
            }
        }

        public void RemoveScene(AbstractScene scene)
        {
            scenes.Remove(scene.GetName());
        }

        /// <summary>
        /// Fully loads the scene, called only once for every scene.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            nextSceneToLoad = scenes[sceneName];
        }

        private void LoadNextScene()
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                AudioEngine.StopSoundEffects();
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
            currentScene.InternalLoad();
            currentScene.ImportData(data);
            currentScene.OnStart();
            isLoading = false;
            useLoadingScreen = false;
        }

        /// <summary>
        /// Starts the current scene. Called every time when the 
        /// scene is becoming the "CurrentScene".
        /// </summary>
        /// <param name="sceneName"></param>
        public void StartScene(string sceneName)
        {
            nextSceneToStart = scenes[sceneName];
        }

        private void StartNextScene()
        {
            ICollection<object> data = null;
            if (currentScene != null)
            {
                AudioEngine.StopSoundEffects();
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

        /// <summary>
        /// Starts the current scene. Called every time when the 
        /// scene is becoming the "CurrentScene".
        /// </summary>
        /// <param name="scene"></param>
        public void StartScene(AbstractScene scene)
        {
            StartScene(scene.GetName());
        }

        /// <summary>
        /// Fully loads the scene, called only once for every scene.
        /// </summary>
        /// <param name="scene"></param>
        public void LoadScene(AbstractScene scene)
        {
            LoadScene(scene.GetName());
        }

        internal bool IsEmpty()
        {
            return scenes.Count == 0;
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
            graphicsDevice.Clear(currentScene.BackgroundColor);
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
