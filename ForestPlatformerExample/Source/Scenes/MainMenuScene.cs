using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.MyGame;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Global;
using MonolithEngine.Source.Camera2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
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
            SelectableImage newGame = new SelectableImage(Assets.GetTexture("HUDNewGameBase"), Assets.GetTexture("HUDNewGameSelected"), new Vector2(150, 150), scale: 0.25f);
            newGame.OnClick = () =>
            {
                SceneManager.LoadScene("Level1");
            };

            SelectableImage settings = new SelectableImage(Assets.GetTexture("HUDSettingsBase"), Assets.GetTexture("HUDSettingsSelected"), new Vector2(150, 200), scale: 0.25f);
            settings.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };

            SelectableImage quit = new SelectableImage(Assets.GetTexture("HUDQuitBase"), Assets.GetTexture("HUDQuitSelected"), new Vector2(150, 250), scale: 0.25f);
            quit.OnClick = Config.ExitAction;

            UI.AddUIElement(quit);
            UI.AddUIElement(settings);
            UI.AddUIElement(newGame);
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            ForestPlatformerGame.Paused = false;
        }
    }
}
