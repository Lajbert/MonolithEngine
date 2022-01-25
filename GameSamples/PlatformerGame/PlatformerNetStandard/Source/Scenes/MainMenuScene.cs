using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    // UI text generated with: https://fontmeme.com/pixel-fonts/
    // font: KA1
    // base color: 2A2A57
    // selected color: FF0000

    class MainMenuScene : AbstractScene
    {

        private LDTKMap world;

        public MainMenuScene(LDTKMap world) : base("MainMenu")
        {
            this.world = world;
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
            Logger.Debug("Loading main menu...");

            Texture2D texture = Assets.GetTexture2D("ForestBG");
            float scale = MonolithGame.Platform.IsMobile() ? 1f : 0.5f;
            Image forestBg = new Image(texture, null, Vector2.Zero, default, scale);
            UI.AddUIElement(forestBg);

            scale = MonolithGame.Platform.IsMobile() ? 8f : 2f;
            Logger.Debug("Loading main menu UI elements...");
            texture = Assets.GetTexture2D("HUDNewGameBase");
            Vector2 newGameButtonPos = MonolithGame.Platform.IsMobile() ? new Vector2(50, 50) : new Vector2(50, 25);
            GameButton newGame = new GameButton(texture, newGameButtonPos, scale: scale, null, true);
            newGame.HoverSoundEffectName = "MenuHover";
            newGame.SelectSoundEffectName = "MenuSelect";

            newGame.OnClick = () =>
            {
#if DEBUG
                SceneManager.StartScene("LevelSelect");
#else
                SceneManager.LoadScene("Level_1");
#endif
            };

            if (MonolithGame.Platform.IsDesktop())
            {
                texture = Assets.GetTexture2D("HUDSettingsBase");
                GameButton settings = new GameButton(texture, new Vector2(50, 50), scale: scale, null, true);
                settings.HoverSoundEffectName = "MenuHover";
                settings.SelectSoundEffectName = "MenuSelect";

                settings.OnClick = () =>
                {
                    SceneManager.StartScene("Settings");
                };

                UI.AddUIElement(settings);

                texture = Assets.GetTexture2D("HUDQuitBase");
                GameButton quit = new GameButton(texture, new Vector2(50, 75), scale: scale, null, true);
                quit.HoverSoundEffectName = "MenuHover";
                quit.SelectSoundEffectName = "MenuSelect";

                PNGFontRenderer fontRenderer = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "quit", default, quit);
                fontRenderer.LetterSpacingOffset = new Vector2(-1, 0);
                fontRenderer.Scale = 1f;
                quit.AddBitmapText(fontRenderer);

                quit.OnClick = Config.ExitAction;

                UI.AddUIElement(quit);

                Camera.Zoom *= 3f;
            }

            LayerManager.Paused = true;

            UI.AddUIElement(newGame);
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            PlatformerGame.Paused = false;
        }

        public override void OnFinished()
        {

        }
    }
}
