using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class GameEndScene : AbstractScene
    {
        public GameEndScene() : base("EndScene", true)
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
            Image congrats = new Image(Assets.GetTexture2D("FinishedText"), new Vector2(150, 150), scale: 0.25f);
            /*
            SelectableImage restart = new SelectableImage(Assets.GetTexture("RestartBase"), Assets.GetTexture("RestartSelected"), new Vector2(150, 200), scale: 0.25f);
            restart.HoverSoundEffectName = "MenuHover";
            restart.SelectSoundEffectName = "MenuSelect";

            restart.OnClick = () =>
            {
                SceneManager.LoadScene("MainMenu");
            };*/

            SelectableImage quit = new SelectableImage(Assets.GetTexture2D("HUDQuitBase"), Assets.GetTexture2D("HUDQuitSelected"), new Vector2(150, 250), scale: 0.25f);
            quit.HoverSoundEffectName = "MenuHover";
            quit.SelectSoundEffectName = "MenuSelect";

            quit.OnClick = Config.ExitAction;

            UI.AddUIElement(congrats);
            UI.AddUIElement(quit);
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
