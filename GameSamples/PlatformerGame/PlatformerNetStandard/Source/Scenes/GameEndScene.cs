using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class GameEndScene : AbstractScene
    {
        public GameEndScene() : base("EndScene", true)
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
            //Image congrats = new Image(Assets.GetTexture2D("FinishedText"), null, new Vector2(20, 20), scale: 0.25f);
            /*
            SelectableImage restart = new SelectableImage(Assets.GetTexture2D("RestartBase"), Assets.GetTexture2D("RestartSelected"), new Vector2(150, 200), scale: 0.25f);
            restart.HoverSoundEffectName = "MenuHover";
            restart.SelectSoundEffectName = "MenuSelect";

            restart.OnClick = () =>
            {
                SceneManager.LoadScene("MainMenu");
            };*/


            PNGFontRenderer fr = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "thank you for playing", new Vector2(50, 50), null);
            fr.LetterSpacingOffset = new Vector2(-1, 0);
            fr.Scale = 6f;
            fr.PositionOffsetPixels = -fr.GetTextDimensions() / 2;
            fr.AnimatedDisplay = true;
            UI.AddUIElement(fr);

            if (MonolithGame.Platform.IsDesktop())
            {
                Texture2D texture = Assets.GetTexture2D("HUDQuitBase");
                GameButton quit = new GameButton(texture, new Vector2(50, 75), scale: 2f, null, true);
                quit.OwnPosition -= new Vector2(texture.Width / 2, texture.Height / 2);
                quit.HoverSoundEffectName = "MenuHover";
                quit.SelectSoundEffectName = "MenuSelect";

                quit.OnClick = Config.ExitAction;

                PNGFontRenderer fontRenderer = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "quit", default, quit);
                fontRenderer.LetterSpacingOffset = new Vector2(-1, 0);
                fontRenderer.Scale = 1f;
                quit.AddBitmapText(fontRenderer);

                UI.AddUIElement(quit);
            }

            //UI.AddUIElement(restart);
        }

        public override void OnEnd()
        {

        }

        public override void OnStart()
        {
            PlatformerGame.Paused = true;
            PlatformerGame.WasGameStarted = false;
        }

        public override void OnFinished()
        {

        }
    }
}
