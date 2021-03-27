using GameEngine2D.Engine.Source.Scene.Transition;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEngine2D.Engine.Source.Scene
{
    public abstract class AbstractScene : Scene
    {

        private SceneManager sceneManager;

        public AbstractScene(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public abstract void Load();

        public abstract void OnEnd();

        public abstract void OnStart();

        public abstract void Unload();

        public abstract ICollection<object> ExportData();

        public void Finish()
        {
            sceneManager.LoadNextScene();
        }

        public abstract ISceneTransitionEffect GetTransitionEffect();

        public abstract void ImportData(ICollection<object> state);
    }
}
