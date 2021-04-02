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
        private MultiSelectionImage resolutionSelect = new MultiSelectionImage(new Vector2(300, 100), scale: 0.25f);
        private MultiSelectionImage frameLimitSelect = new MultiSelectionImage(new Vector2(300, 200), scale: 0.25f);
        private MultiSelectionImage vsyncSelect = new MultiSelectionImage(new Vector2(300, 300), scale: 0.25f);
        private MultiSelectionImage windowModeSelect = new MultiSelectionImage(new Vector2(300, 400), scale: 0.25f);

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
            Image resolutionLabel = new Image(Assets.GetTexture("HUDResolutionLabel"), new Vector2(150, 50), scale: 0.25f);
            resolutionSelect.AddOption("720p", Assets.GetTexture("HUD720p"));
            resolutionSelect.AddOption("1080p", Assets.GetTexture("HUD1080p"));
            resolutionSelect.AddOption("1440p", Assets.GetTexture("HUD1440p"));
            resolutionSelect.AddOption("4K", Assets.GetTexture("HUD4K"));
            SelectableImage resolutionRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(485, 100), scale: 0.02f);
            resolutionRight.OnClick = () =>
            {
                resolutionSelect.Next();
            };
            SelectableImage resolutionLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(265, 100), scale: 0.02f);
            resolutionLeft.OnClick = () =>
            {
                resolutionSelect.Previous();
            };


            Image frameLimiterLabel = new Image(Assets.GetTexture("HUDFPSLimitLabel"), new Vector2(150, 150), scale: 0.25f);
            frameLimitSelect.AddOption("30", Assets.GetTexture("HUD30"));
            frameLimitSelect.AddOption("60", Assets.GetTexture("HUD60"));
            frameLimitSelect.AddOption("120", Assets.GetTexture("HUD120"));
            frameLimitSelect.AddOption("Unlimited", Assets.GetTexture("HUDUnlimited"));

            SelectableImage fpsRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(485, 200), scale: 0.02f);
            fpsRight.OnClick = () =>
            {
                frameLimitSelect.Next();
            };
            SelectableImage fpsLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(265, 200), scale: 0.02f);
            fpsLeft.OnClick = () =>
            {
                frameLimitSelect.Previous();
            };

            Image vsyncLabel = new Image(Assets.GetTexture("HUDVsyncLabel"), new Vector2(150, 250), scale: 0.25f);
            vsyncSelect.AddOption("On", Assets.GetTexture("HUDOn"));
            vsyncSelect.AddOption("Off", Assets.GetTexture("HUDOff"));

            SelectableImage vsyncRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(485, 300), scale: 0.02f);
            vsyncRight.OnClick = () =>
            {
                vsyncSelect.Next();
            };
            SelectableImage vsyncLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(265, 300), scale: 0.02f);
            vsyncLeft.OnClick = () =>
            {
                vsyncSelect.Previous();
            };

            Image windowModeLabel = new Image(Assets.GetTexture("HUDWindowModeLabel"), new Vector2(150, 350), scale: 0.25f);
            windowModeSelect.AddOption("Fullscreen", Assets.GetTexture("HUDFullscreen"));
            windowModeSelect.AddOption("Windowed", Assets.GetTexture("HUDWindowed"));

            SelectableImage windowModeRight = new SelectableImage(Assets.GetTexture("HUDArrowRightBase"), Assets.GetTexture("HUDArrowRightSelected"), new Vector2(485, 400), scale: 0.02f);
            windowModeRight.OnClick = () =>
            {
                windowModeSelect.Next();
            };
            SelectableImage windowModeLeft = new SelectableImage(Assets.GetTexture("HUDArrowLeftBase"), Assets.GetTexture("HUDArrowLeftSelected"), new Vector2(265, 400), scale: 0.02f);
            windowModeLeft.OnClick = () =>
            {
                windowModeSelect.Previous();
            };

            SelectableImage cancel = new SelectableImage(Assets.GetTexture("HUDCancelBase"), Assets.GetTexture("HUDCancelSelected"), new Vector2(150, 500), scale: 0.25f);
            cancel.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };
            SelectableImage apply = new SelectableImage(Assets.GetTexture("HUDApplyBase"), Assets.GetTexture("HUDApplySelected"), new Vector2(450, 500), scale: 0.25f);
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
            if (resolutionSelect.GetSelection().Equals("720p")) {
                VideoConfiguration.RESOLUTION_WIDTH = 1280;
                VideoConfiguration.RESOLUTION_HEIGHT = 720;
            } 
            else if (resolutionSelect.GetSelection().Equals("1080p")) {
                VideoConfiguration.RESOLUTION_WIDTH = 1920;
                VideoConfiguration.RESOLUTION_HEIGHT = 1080;
            }
            else if(resolutionSelect.GetSelection().Equals("1440p")) {
                VideoConfiguration.RESOLUTION_WIDTH = 2560;
                VideoConfiguration.RESOLUTION_HEIGHT = 1440;
            }
            else if(resolutionSelect.GetSelection().Equals("4K")) {
                VideoConfiguration.RESOLUTION_WIDTH = 3840;
                VideoConfiguration.RESOLUTION_HEIGHT = 2160;
            } 

            if (vsyncSelect.GetSelection().Equals("On"))
            {
                VideoConfiguration.VSYNC = true;
            } else
            {
                VideoConfiguration.VSYNC = false;
            }

            if (windowModeSelect.GetSelection().Equals("Fullscreen"))
            {
                VideoConfiguration.FULLSCREEN = true;
            } else
            {
                VideoConfiguration.FULLSCREEN = false;
            }

            if (frameLimitSelect.GetSelection().Equals("Unlimited"))
            {
                VideoConfiguration.FRAME_LIMIT = 0;
            }
            else
            {
                VideoConfiguration.FRAME_LIMIT = int.Parse(frameLimitSelect.GetSelection());
            }

            VideoConfiguration.Apply();

            Camera.ResolutionUpdated();

            SceneManager.OnResolutionChanged();
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
