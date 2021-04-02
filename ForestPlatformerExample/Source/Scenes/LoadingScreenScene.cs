using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class LoadingScreenScene : AbstractScene
    {
        public LoadingScreenScene() : base("loading", true)
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
            Image loadingImage = new Image(Assets.GetTexture("HUDLoading"), new Vector2(200, 200));
            
            UI.AddUIElement(loadingImage);
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            
        }
    }
}
