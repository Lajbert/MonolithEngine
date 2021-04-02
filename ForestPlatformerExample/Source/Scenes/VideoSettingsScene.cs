using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.MyGame;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class VideoSettingsScene : AbstractScene
    {
        private MultiSelectionImage resolutionSelect = new MultiSelectionImage(new Vector2(600, 400), scale: 0.25f);
        private MultiSelectionImage frameLimitSelect = new MultiSelectionImage(new Vector2(600, 700), scale: 0.25f);
        private MultiSelectionImage vsyncSelect = new MultiSelectionImage(new Vector2(600, 900), scale: 0.25f);
        private MultiSelectionImage windowModeSelect = new MultiSelectionImage(new Vector2(600, 1100), scale: 0.25f);

        public VideoSettingsScene() : base ("VideoSettings", preload: true)
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
            Image resolutionLabel = new Image(Assets.GetTexture("HUDResolutionLabel"), new Vector2(300, 300), scale: 0.25f);
            resolutionSelect.AddOption("720p", Assets.GetTexture("HUD720p"));
            resolutionSelect.AddOption("1080p", Assets.GetTexture("HUD1080p"));
            resolutionSelect.AddOption("1440p", Assets.GetTexture("HUD1440p"));
            resolutionSelect.AddOption("4K", Assets.GetTexture("HUD4K"));
            SelectableImage resolutionRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(1370, 400), scale: 0.02f);
            resolutionRight.OnClick = () =>
            {
                resolutionSelect.Next();
            };
            SelectableImage resolutionLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(530, 400), scale: 0.02f);
            resolutionLeft.OnClick = () =>
            {
                resolutionSelect.Previous();
            };


            Image frameLimiterLabel = new Image(Assets.GetTexture("HUDFPSLimitLabel"), new Vector2(300, 600), scale: 0.25f);
            frameLimitSelect.AddOption("30", Assets.GetTexture("HUD30"));
            frameLimitSelect.AddOption("60", Assets.GetTexture("HUD60"));
            frameLimitSelect.AddOption("120", Assets.GetTexture("HUD120"));
            frameLimitSelect.AddOption("Unlimited", Assets.GetTexture("HUDUnlimited"));

            SelectableImage fpsRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(1370, 700), scale: 0.02f);
            fpsRight.OnClick = () =>
            {
                frameLimitSelect.Next();
            };
            SelectableImage fpsLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(530, 700), scale: 0.02f);
            fpsLeft.OnClick = () =>
            {
                frameLimitSelect.Previous();
            };

            Image vsyncLabel = new Image(Assets.GetTexture("HUDVsyncLabel"), new Vector2(300, 800), scale: 0.25f);
            vsyncSelect.AddOption("On", Assets.GetTexture("HUDOn"));
            vsyncSelect.AddOption("Off", Assets.GetTexture("HUDOff"));

            SelectableImage vsyncRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(1370, 900), scale: 0.02f);
            vsyncRight.OnClick = () =>
            {
                vsyncSelect.Next();
            };
            SelectableImage vsyncLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(530, 900), scale: 0.02f);
            vsyncLeft.OnClick = () =>
            {
                vsyncSelect.Previous();
            };

            Image windowModeLabel = new Image(Assets.GetTexture("HUDWindowModeLabel"), new Vector2(300, 1000), scale: 0.25f);
            windowModeSelect.AddOption("Fullscreen", Assets.GetTexture("HUDFullscreen"));
            windowModeSelect.AddOption("Windowed", Assets.GetTexture("HUDWindowed"));

            SelectableImage windowModeRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(1370, 1100), scale: 0.02f);
            windowModeRight.OnClick = () =>
            {
                windowModeSelect.Next();
            };
            SelectableImage windowModeLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(530, 1100), scale: 0.02f);
            windowModeLeft.OnClick = () =>
            {
                windowModeSelect.Previous();
            };

            SelectableImage cancel = new SelectableImage(Assets.GetTexture("HUDCancelBase"), Assets.GetTexture("HUDCancelSelected"), new Vector2(300, 1300), scale: 0.25f);
            cancel.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };
            SelectableImage apply = new SelectableImage(Assets.GetTexture("HUDApplyBase"), Assets.GetTexture("HUDApplySelected"), new Vector2(900, 1300), scale: 0.25f);
            apply.OnClick = ApplyConfiguration;

            UI.AddUIElement(resolutionLabel);
            UI.AddUIElement(frameLimiterLabel);
            UI.AddUIElement(vsyncLabel);
            UI.AddUIElement(windowModeLabel);
            UI.AddUIElement(resolutionSelect);
            UI.AddUIElement(frameLimitSelect);
            UI.AddUIElement(vsyncSelect);
            UI.AddUIElement(windowModeSelect);
            UI.AddUIElement(resolutionRight);
            UI.AddUIElement(resolutionLeft);
            UI.AddUIElement(fpsLeft);
            UI.AddUIElement(fpsRight);
            UI.AddUIElement(vsyncLeft);
            UI.AddUIElement(vsyncRight);
            UI.AddUIElement(windowModeRight);
            UI.AddUIElement(windowModeLeft);
            UI.AddUIElement(apply);
            UI.AddUIElement(cancel);

            SetCurrentVideoSettings();
        }

        private void SetCurrentVideoSettings()
        {
            if (VideoConfiguration.RESOLUTION_HEIGHT == 720)
            {
                resolutionSelect.SetSelected("720p");
            } 
            else if (VideoConfiguration.RESOLUTION_HEIGHT == 1080)
            {
                resolutionSelect.SetSelected("1080p");
            }
            else if (VideoConfiguration.RESOLUTION_HEIGHT == 1440)
            {
                resolutionSelect.SetSelected("1440p");
            }
            else if (VideoConfiguration.RESOLUTION_HEIGHT == 2160)
            {
                resolutionSelect.SetSelected("4K");
            }

            if (VideoConfiguration.FRAME_LIMIT == 0)
            {
                frameLimitSelect.SetSelected("Unlimited");
            } 
            else if (VideoConfiguration.FRAME_LIMIT == 30)
            {
                frameLimitSelect.SetSelected("30");
            } 
            else if (VideoConfiguration.FRAME_LIMIT == 60)
            {
                frameLimitSelect.SetSelected("60");
            }
            else if (VideoConfiguration.FRAME_LIMIT == 120)
            {
                frameLimitSelect.SetSelected("120");
            }

            if (VideoConfiguration.VSYNC)
            {
                vsyncSelect.SetSelected("On");
            } 
            else
            {
                vsyncSelect.SetSelected("Off");
            }

            if (VideoConfiguration.FULLSCREEN)
            {
                windowModeSelect.SetSelected("Fullscreen");
            } 
            else
            {
                windowModeSelect.SetSelected("Windowed");
            }
        }

        public void ApplyConfiguration()
        {

        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            ForestPlatformerGame.Paused = true;
            ForestPlatformerGame.GameRunning = false;
            SetCurrentVideoSettings();
        }
    }
}
