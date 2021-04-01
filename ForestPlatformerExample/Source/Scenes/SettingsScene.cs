using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class SettingsScene : AbstractScene
    {
        public SettingsScene() : base("Settings")
        {

        }

        public override ICollection<object> ExportData()
        {
            return null;
        }

        public override ISceneTransitionEffect GetTransitionEffect()
        {
            return null;
        }

        public override void ImportData(ICollection<object> state)
        {
            
        }

        public override void Load()
        {
            
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            ForestPlatformerGame.Paused = true;
            ForestPlatformerGame.GameRunning = false;
        }
    }
}
