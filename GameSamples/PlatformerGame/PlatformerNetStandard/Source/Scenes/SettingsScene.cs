using Microsoft.Xna.Framework;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
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

            SelectableImage videoSettings = new SelectableImage(Assets.GetTexture2D("HUDVideoSettingsBase"), Assets.GetTexture2D("HUDVideoSettingsSelected"), null, new Vector2(20, 20), scale: 0.25f);
            videoSettings.HoverSoundEffectName = "MenuHover";
            videoSettings.SelectSoundEffectName = "MenuSelect";

            videoSettings.OnClick = () =>
            {
                SceneManager.StartScene("VideoSettings");
            };

            SelectableImage audioSettings = new SelectableImage(Assets.GetTexture2D("HUDAudioSettingsBase"), Assets.GetTexture2D("HUDAudioSettingsSelected"), null, new Vector2(20, 30), scale: 0.25f);
            audioSettings.HoverSoundEffectName = "MenuHover";
            audioSettings.SelectSoundEffectName = "MenuSelect";

            audioSettings.OnClick = () =>
            {
                SceneManager.StartScene("AudioSettings");
            };

            GameButton back = new GameButton(Assets.GetTexture2D("HUDBackBase"), new Vector2(20, 40), scale: 0.25f);
            //SelectableImage back = new SelectableImage(Assets.GetTexture2D("HUDBackBase"), Assets.GetTexture2D("HUDBackSelected"), null, new Vector2(20, 40), scale: 0.25f);
            back.HoverSoundEffectName = "MenuHover";
            back.SelectSoundEffectName = "MenuSelect";

            back.OnClick = () =>
            {
                if (PlatformerGame.WasGameStarted)
                {
                    SceneManager.StartScene("PauseMenu");
                } else
                {
                    SceneManager.StartScene("MainMenu");
                }
                
            };

            UI.AddUIElement(videoSettings);
            //UI.AddUIElement(audioSettings);
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
