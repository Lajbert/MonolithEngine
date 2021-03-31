using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
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
    class PauseMenuScene : AbstractScene
    {
        public PauseMenuScene(Camera camera) : base(camera, "PauseMenu", true)
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
            SelectableImage continueGame = new SelectableImage(Assets.GetTexture("HUDContinueBase"), Assets.GetTexture("HUDContinueSelected"), new Vector2(300, 300), scale: 1);
            continueGame.OnClick = () =>
            {
                SceneManager.StartScene("Level1");
            };

            SelectableImage quit = new SelectableImage(Assets.GetTexture("HUDQuitBase"), Assets.GetTexture("HUDQuitSelected"), new Vector2(300, 500), scale: 1);
            quit.OnClick = Config.ExitAction;

            UI.AddUIElement(quit);
            UI.AddUIElement(continueGame);
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
