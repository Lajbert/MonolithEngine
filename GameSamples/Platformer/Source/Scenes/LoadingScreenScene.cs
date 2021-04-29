using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class LoadingScreenScene : AbstractScene
    {
        public LoadingScreenScene() : base("loading", preload: true)
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

        public override void OnFinished()
        {

        }
    }
}
