using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class LevelSelectScreen : AbstractScene
    {
        public LevelSelectScreen() : base("LevelSelect", preload: true)
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
            float scale = PlatformerGame.ANDROID ? 0.5f : 0.25f;
            Logger.Debug("Loading level select scene UI...");
            SelectableImage videoSettings = new SelectableImage(Assets.GetTexture("Level1Base"), Assets.GetTexture("Level1Selected"), new Vector2(150, 150), scale: scale);
            videoSettings.HoverSoundEffectName = "MenuHover";
            videoSettings.SelectSoundEffectName = "MenuSelect";

            videoSettings.OnClick = () =>
            {
                SceneManager.LoadScene("Level_1");
            };

            SelectableImage audioSettings = new SelectableImage(Assets.GetTexture("Level2Base"), Assets.GetTexture("Level2Selected"), new Vector2(150, 250), scale: scale);
            audioSettings.HoverSoundEffectName = "MenuHover";
            audioSettings.SelectSoundEffectName = "MenuSelect";

            audioSettings.OnClick = () =>
            {
                SceneManager.LoadScene("Level_2");
            };

            SelectableImage back = new SelectableImage(Assets.GetTexture("HUDBackBase"), Assets.GetTexture("HUDBackSelected"), new Vector2(150, 350), scale: scale);
            back.HoverSoundEffectName = "MenuHover";
            back.SelectSoundEffectName = "MenuSelect";

            back.OnClick = () =>
            {
                SceneManager.LoadScene("MainMenu");
            };

            UI.AddUIElement(videoSettings);
            UI.AddUIElement(audioSettings);
            UI.AddUIElement(back);
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
