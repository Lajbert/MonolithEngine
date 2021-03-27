using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Scene
{
    public class SceneManager
    {
        private List<AbstractScene> scenes = new List<AbstractScene>();
        private AbstractScene currentScene;
        private int currentSceneIndex;

        public void LoadNextScene()
        {
            LoadScene(currentSceneIndex++);
        }

        public void LoadScene(int sceneIndex)
        {
            ICollection<object> data = currentScene.ExportData();
            currentScene.OnEnd();
            currentScene.Unload();
            currentScene = scenes[sceneIndex];
            currentScene.Load();
            currentScene.ImportData(data);
            currentScene.OnStart();
        }

        public void LoadScene(AbstractScene scene)
        {
            LoadScene(scenes.IndexOf(scene));
        }

    }
}
