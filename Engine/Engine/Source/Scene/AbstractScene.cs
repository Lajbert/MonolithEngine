using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    public abstract class AbstractScene : IScene
    {

        protected SceneManager SceneManager;

        protected string SceneName;

        internal bool Preload = false;

        internal bool AlwaysActive;

        protected UserInterface UI;

        public LayerManager LayerManager { get; }

        public CollisionEngine CollisionEngine;

        public GridCollisionChecker GridCollisionChecker;

        public Camera Camera;

        public bool UseLoadingScreen;

        public Color BackgroundColor = Color.White;

        public AbstractScene(string sceneName, bool preload = false, bool alwaysActive = false, bool useLoadingScreen = false)
        {
            if (sceneName == null || sceneName.Length == 0)
            {
                throw new Exception("The scene must have a non-null, non-empty unique name!");
            }
            this.SceneName = sceneName;
            Preload = preload;
            AlwaysActive = alwaysActive;
            UseLoadingScreen = useLoadingScreen;
            UI = new UserInterface();

            LayerManager = new LayerManager(this);

            CollisionEngine = new CollisionEngine();

            GridCollisionChecker = new GridCollisionChecker();

        }

        internal void InternalLoad()
        {
            LayerManager.InitLayers();
            Load();
            UI.HandleNewElements();
        }

        public abstract void Load();

        public abstract void OnEnd();

        public abstract void OnStart();

        public abstract void OnFinished();

        public virtual void Unload()
        {
            LayerManager.Destroy();
            CollisionEngine.Destroy();
            GridCollisionChecker.Destroy();
            UI.Clear();
            Timer.Clear();
        }

        public abstract ICollection<object> ExportData();

        public void Finish()
        {
            SceneManager.OnSceneFinished(this);
            OnFinished();
        }

        public void OnResolitionChanged()
        {
            UI.OnResolutionChanged();
        }

        public abstract ISceneTransitionEffect GetTransitionEffect();

        public abstract void ImportData(ICollection<object> state);

        internal void SetSceneManager(SceneManager sceneManager)
        {
            this.SceneManager = sceneManager;
        }

        public string GetName()
        {
            return SceneName;
        }

        public virtual void Update()
        {
            LayerManager.UpdateAll();
            UI.Update();
        }

        public void FixedUpdate()
        {
            LayerManager.FixedUpdateAll();
            //CollisionEngine.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LayerManager.DrawAll(spriteBatch);

            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.GetUITransformMatrix());
            UI.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
