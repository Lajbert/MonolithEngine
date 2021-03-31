﻿using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.Physics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Entities;
using MonolithEngine.Source.Camera2D;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MonolithEngine.Engine.Source.Scene
{
    public abstract class AbstractScene : IScene
    {

        protected SceneManager SceneManager;

        private string sceneName;

        internal bool Preload = false;

        internal bool AlwaysActive;

        protected UserInterface UI;

        public LayerManager LayerManager { get; }

        public CollisionEngine CollisionEngine;

        public GridCollisionChecker GridCollisionChecker;

        public Camera Camera;

        public AbstractScene(Camera camera, string sceneName, bool preload = false, bool alwaysActive = false)
        {
            if (sceneName == null || sceneName.Length == 0)
            {
                throw new Exception("The scene must have a non-null, non-empty unique name!");
            }
            this.sceneName = sceneName;
            Preload = preload;
            AlwaysActive = alwaysActive;

            UI = new UserInterface();

            LayerManager = new LayerManager(this);
            LayerManager.InitLayers();

            CollisionEngine = new CollisionEngine();

            GridCollisionChecker = new GridCollisionChecker();

            Camera = camera;
        }

        public abstract void Load();

        public abstract void OnEnd();

        public abstract void OnStart();

        public virtual void Unload()
        {
            LayerManager.Destroy();
            CollisionEngine.Destroy();
            GridCollisionChecker.Destroy();
        }

        public abstract ICollection<object> ExportData();

        public void Finish()
        {
            SceneManager.OnSceneFinished(this);
        }

        public abstract ISceneTransitionEffect GetTransitionEffect();

        public abstract void ImportData(ICollection<object> state);

        internal void SetSceneManager(SceneManager sceneManager)
        {
            this.SceneManager = sceneManager;
        }

        public string GetName()
        {
            return sceneName;
        }

        public virtual void Update()
        {
            LayerManager.UpdateAll();
            UI.Update();
        }

        public void FixedUpdate()
        {
            LayerManager.FixedUpdateAll();
            CollisionEngine.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LayerManager.DrawAll(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            UI.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}