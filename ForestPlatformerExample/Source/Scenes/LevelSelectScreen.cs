using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
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

            SelectableImage videoSettings = new SelectableImage(Assets.GetTexture("Level1Base"), Assets.GetTexture("Level1Selected"), new Vector2(150, 150), scale: 0.25f);
            videoSettings.HoverSoundEffectName = "MenuHover";
            videoSettings.SelectSoundEffectName = "MenuSelect";

            videoSettings.OnClick = () =>
            {
                SceneManager.LoadScene("Level_1");
            };

            SelectableImage audioSettings = new SelectableImage(Assets.GetTexture("Level2Base"), Assets.GetTexture("Level2Selected"), new Vector2(150, 200), scale: 0.25f);
            audioSettings.HoverSoundEffectName = "MenuHover";
            audioSettings.SelectSoundEffectName = "MenuSelect";

            audioSettings.OnClick = () =>
            {
                SceneManager.LoadScene("Level_2");
            };

            SelectableImage back = new SelectableImage(Assets.GetTexture("HUDBackBase"), Assets.GetTexture("HUDBackSelected"), new Vector2(150, 250), scale: 0.25f);
            back.HoverSoundEffectName = "MenuHover";
            back.SelectSoundEffectName = "MenuSelect";

            back.OnClick = () =>
            {
                if (ForestPlatformerGame.WasGameStarted)
                {
                    SceneManager.StartScene("PauseMenu");
                }
                else
                {
                    SceneManager.StartScene("MainMenu");
                }

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
            ForestPlatformerGame.Paused = true;
        }

        public override void OnFinished()
        {

        }
    }
}
