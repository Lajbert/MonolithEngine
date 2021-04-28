using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class PauseMenuScene : AbstractScene
    {
        public PauseMenuScene() : base("PauseMenu", true)
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
            SelectableImage continueGame = new SelectableImage(Assets.GetTexture("HUDContinueBase"), Assets.GetTexture("HUDContinueSelected"), new Vector2(150, 150), scale: 0.25f);
            continueGame.HoverSoundEffectName = "MenuHover";
            continueGame.SelectSoundEffectName = "MenuSelect";

            continueGame.OnClick = () =>
            {
                SceneManager.StartScene("Level_1");
            };

            SelectableImage settings = new SelectableImage(Assets.GetTexture("HUDSettingsBase"), Assets.GetTexture("HUDSettingsSelected"), new Vector2(150, 200), scale: 0.25f);
            settings.HoverSoundEffectName = "MenuHover";
            settings.SelectSoundEffectName = "MenuSelect";

            settings.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };

            SelectableImage quit = new SelectableImage(Assets.GetTexture("HUDQuitBase"), Assets.GetTexture("HUDQuitSelected"), new Vector2(150, 250), scale: 0.25f);
            quit.HoverSoundEffectName = "MenuHover";
            quit.SelectSoundEffectName = "MenuSelect";

            quit.OnClick = Config.ExitAction;

            UI.AddUIElement(quit);
            UI.AddUIElement(continueGame);
            UI.AddUIElement(settings);
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            ForestPlatformerGame.Paused = true;
        }

        public override void OnFinished()
        {

        }
    }
}
