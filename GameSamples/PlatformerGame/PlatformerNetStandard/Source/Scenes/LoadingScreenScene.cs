using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class LoadingScreenScene : AbstractScene
    {
        public LoadingScreenScene() : base("loading", preload: true)
        {
            BackgroundColor = Color.Black;
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
            //Image loadingImage = new Image(Assets.GetTexture2D("HUDLoading"), null, new Vector2(30, 40));

            //UI.AddUIElement(loadingImage);

            PNGFontSheet font = Assets.GetPNGFontSheet("PixelFont");
            PNGFontRenderer fontRenderer = new PNGFontRenderer(font, "loading", new Vector2(50, 50));
            fontRenderer.LetterSpacingOffset = new Vector2(-1, 0);
            fontRenderer.Scale = 15f;
            fontRenderer.PositionOffsetPixels -= (fontRenderer.GetTextDimensions() / 2);
            UI.AddUIElement(fontRenderer);
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
