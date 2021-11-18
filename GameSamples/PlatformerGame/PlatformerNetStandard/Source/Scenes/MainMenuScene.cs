using Microsoft.Xna.Framework;
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
        public MainMenuScene() : base("MainMenu")
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
            float scale = MonolithGame.Platform.IsMobile() ? 1f : 0.25f;
            Logger.Debug("Loading main menu UI elements...");
            SelectableImage newGame = new SelectableImage(Assets.GetTexture2D("HUDNewGameBase"), Assets.GetTexture2D("HUDNewGameSelected"), new Vector2(150, 150), scale: scale);
            newGame.HoverSoundEffectName = "MenuHover";
            newGame.SelectSoundEffectName = "MenuSelect";

            newGame.OnClick = () =>
            {
                SceneManager.LoadScene("LevelSelect");
            };

            SelectableImage settings = null;
            if (!MonolithGame.Platform.IsMobile())
            {
                settings = new SelectableImage(Assets.GetTexture2D("HUDSettingsBase"), Assets.GetTexture2D("HUDSettingsSelected"), new Vector2(150, 200), scale: scale);
                settings.HoverSoundEffectName = "MenuHover";
                settings.SelectSoundEffectName = "MenuSelect";

                settings.OnClick = () =>
                {
                    SceneManager.StartScene("Settings");
                };
            }


            SelectableImage quit = new SelectableImage(Assets.GetTexture2D("HUDQuitBase"), Assets.GetTexture2D("HUDQuitSelected"), new Vector2(150, 250), scale: scale);
            quit.HoverSoundEffectName = "MenuHover";
            quit.SelectSoundEffectName = "MenuSelect";

            quit.OnClick = Config.ExitAction;

            UI.AddUIElement(quit);
            if (!MonolithGame.Platform.IsMobile())
            {
                UI.AddUIElement(settings);
            }

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
