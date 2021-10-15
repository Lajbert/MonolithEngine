using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Base class representing a scene (level, area, menu, etc). 
    /// Automatically loads and unloads components,
    /// makes organizing the game much easier.
    /// There is always one active scene in the game (called CurrentScene),
    /// but other scenes might also be updated in the background
    /// even when they are not the active one.
    /// For example, in a platformer game, you don't need the other scenes to be updated
    /// but the active one, while in a top-down RPG, when you are in a tavern which is the
    /// active scene, you might still want the rest of the town to be updated with the 
    /// NPCs doing their daily routine, day-night cycle, etc.
    /// </summary>
    public abstract class AbstractScene : IScene
    {

        protected SceneManager SceneManager;

        protected string SceneName;

        // true: we load the scene at the game's startup
        // false: we load the scene only we load it
        internal bool Preload = false;

        // true: the scene is being updated even when it's not the current scene
        // false: the scene is not upated when it's not the current scene
        internal bool AlwaysActive;

        protected UserInterface UI;

        public LayerManager LayerManager { get; }

        public CollisionEngine CollisionEngine;

        public GridCollisionChecker GridCollisionChecker;

        public List<Camera> Cameras;

        public Camera CurrentCamera;

        // true: when loading the scene, a static, preconfigured loading screen appears
        // false; we load the scene without a loading screen
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

        /// <summary>
        /// Called once when current scene is loaded. Do all your hardware heavy loading 
        /// stuff here: assets (textures, entities, etc). Loading screens can be displayed here.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Called when the scene is ended during scene transitions.
        /// Called every time when the scene is not the active scene anymore.
        /// Only do lighweight operations here, that is absolutely necessary
        /// when transitioning away from this scene.
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// Called when the scene is started during scene transitions.
        /// Called every time when the scene is the active scene again.
        /// Only do lighweight operations here, that is absolutely necessary
        /// when the scene becomes the active scene again.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Called once when the scene is finished.
        /// </summary>
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
            CollisionEngine.PostUpdate();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LayerManager.DrawAll(spriteBatch);
            foreach (Camera camera in Cameras)
            {
                CurrentCamera = camera;
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.GetUITransformMatrix());
                UI.Draw(spriteBatch);
                spriteBatch.End();
            }
        }
    }
}
