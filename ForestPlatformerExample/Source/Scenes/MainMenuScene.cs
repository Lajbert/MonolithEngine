using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.MyGame;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
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
        public MainMenuScene(Camera camera) : base(camera, "MainMenu")
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
            //UI.AddUIElement(new Image(Assets.GetTexture("HUDCointCount"), new Vector2(30, 30), scale: 8));
            SelectableImage newGame = new SelectableImage(Assets.GetTexture("HUDNewGameBase"), Assets.GetTexture("HUDNewGameSelected"), new Vector2(300, 300), scale: 1);
            newGame.OnClick = () =>
            {
                SceneManager.LoadScene("Level1");
            };

            SelectableImage quit = new SelectableImage(Assets.GetTexture("HUDQuitBase"), Assets.GetTexture("HUDQuitSelected"), new Vector2(300, 500), scale: 1);
            quit.OnClick = SceneManager.ExitAction;

            UI.AddUIElement(quit);
            UI.AddUIElement(newGame);
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            
        }
    }
}
