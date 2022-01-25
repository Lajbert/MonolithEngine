using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
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
            SelectableImage continueGame = new SelectableImage(Assets.GetTexture2D("ContinueButton"), Assets.GetTexture2D("ContinueButton"), null, new Vector2(20, 20), scale: 0.25f);
            continueGame.HoverSoundEffectName = "MenuHover";
            continueGame.SelectSoundEffectName = "MenuSelect";

            continueGame.OnClick = () =>
            {
                SceneManager.StartScene(PlatformerGame.CurrentScene);
            };

            SelectableImage settings = new SelectableImage(Assets.GetTexture2D("HUDSettingsBase"), Assets.GetTexture2D("HUDSettingsSelected"), null, new Vector2(20, 30), scale: 0.25f);
            settings.HoverSoundEffectName = "MenuHover";
            settings.SelectSoundEffectName = "MenuSelect";

            settings.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };

            SelectableImage quit = new SelectableImage(Assets.GetTexture2D("HUDQuitBase"), Assets.GetTexture2D("HUDQuitBase"), null, new Vector2(20, 30), scale: 0.25f);
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
            PlatformerGame.Paused = true;
        }

        public override void OnFinished()
        {

        }
    }
}
