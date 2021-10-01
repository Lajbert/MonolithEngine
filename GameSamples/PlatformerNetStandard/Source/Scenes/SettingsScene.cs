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

            SelectableImage videoSettings = new SelectableImage(Assets.GetTexture("HUDVideoSettingsBase"), Assets.GetTexture("HUDVideoSettingsSelected"), new Vector2(150, 150), scale: 0.25f);
            videoSettings.HoverSoundEffectName = "MenuHover";
            videoSettings.SelectSoundEffectName = "MenuSelect";

            videoSettings.OnClick = () =>
            {
                SceneManager.StartScene("VideoSettings");
            };

            SelectableImage audioSettings = new SelectableImage(Assets.GetTexture("HUDAudioSettingsBase"), Assets.GetTexture("HUDAudioSettingsSelected"), new Vector2(150, 200), scale: 0.25f);
            audioSettings.HoverSoundEffectName = "MenuHover";
            audioSettings.SelectSoundEffectName = "MenuSelect";

            audioSettings.OnClick = () =>
            {
                SceneManager.StartScene("AudioSettings");
            };

            SelectableImage back = new SelectableImage(Assets.GetTexture("HUDBackBase"), Assets.GetTexture("HUDBackSelected"), new Vector2(150, 250), scale: 0.25f);
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
