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
    class SettingsScene : AbstractScene
    {
        public SettingsScene() : base("Settings", preload: true)
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

            SelectableImage videoSettings = new SelectableImage(Assets.GetTexture("HUDVideoSettingsBase"), Assets.GetTexture("HUDVideoSettingsSelected"), new Vector2(300, 300), scale: 0.25f);
            videoSettings.OnClick = () =>
            {
                SceneManager.StartScene("VideoSettings");
            };

            SelectableImage audioSettings = new SelectableImage(Assets.GetTexture("HUDAudioSettingsBase"), Assets.GetTexture("HUDAudioSettingsSelected"), new Vector2(300, 400), scale: 0.25f);
            audioSettings.OnClick = () =>
            {
                SceneManager.StartScene("AudioSettings");
            };

            SelectableImage back = new SelectableImage(Assets.GetTexture("HUDBackBase"), Assets.GetTexture("HUDBackSelected"), new Vector2(300, 500), scale: 0.25f);
            back.OnClick = () =>
            {
                if (ForestPlatformerGame.WasGameStarted)
                {
                    SceneManager.StartScene("PauseMenu");
                } else
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
            ForestPlatformerGame.GameRunning = false;
        }
    }
}
