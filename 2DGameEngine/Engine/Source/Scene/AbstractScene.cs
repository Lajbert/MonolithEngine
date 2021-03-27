using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MonolithEngine.Engine.Source.Scene
{
    public abstract class AbstractScene : IScene
    {

        private SceneManager sceneManager;

        private string sceneName;

        internal bool Preload = false;

        protected UserInterface UI;

        public AbstractScene(string sceneName, bool preload = false)
        {
            if (sceneName == null || sceneName.Length == 0)
            {
                throw new Exception("The scene must have a non-null, non-empty unique name!");
            }
            this.sceneName = sceneName;
            Preload = preload;

            UI = new UserInterface();
        }

        public abstract void Load();

        public abstract void OnEnd();

        public abstract void OnStart();

        public abstract void Unload();

        public abstract ICollection<object> ExportData();

        public void Finish()
        {
            sceneManager.OnSceneFinished(this);
        }

        public abstract ISceneTransitionEffect GetTransitionEffect();

        public abstract void ImportData(ICollection<object> state);

        internal void SetSceneManager(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public string GetName()
        {
            return sceneName;
        }

        public virtual void Update()
        {
            UI.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            UI.Draw(spriteBatch);
        }
    }
}
