using Microsoft.Xna.Framework;
using MonolithEngine;
using System;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class VideoSettingsScene : AbstractScene
    {

        private static readonly int LABEL_POSITION = 30;
        private static readonly int SELECTION_POSITION = LABEL_POSITION + 3;
        private static readonly int LEFT_ARROW = SELECTION_POSITION - 2;
        private static readonly int RIGHT_ARROW = SELECTION_POSITION + 20;
        private int currentYCoord;

        private MultiSelectionImage resolutionSelect;
        private MultiSelectionImage frameLimitSelect;
        private MultiSelectionImage vsyncSelect;
        private MultiSelectionImage windowModeSelect;

        public VideoSettingsScene() : base ("VideoSettings", preload: true)
        {
            currentYCoord = 0;
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
            Image resolutionLabel = new Image(Assets.GetTexture2D("HUDResolutionLabel"), null, new Vector2(LABEL_POSITION, NextPosition()), scale: 0.25f);
            resolutionSelect = new MultiSelectionImage(null, new Vector2(SELECTION_POSITION, NextPosition()), scale: 0.25f);
            resolutionSelect.AddOption("720p", Assets.GetTexture2D("HUD720p"));
            resolutionSelect.AddOption("1080p", Assets.GetTexture2D("HUD1080p"));
            resolutionSelect.AddOption("1440p", Assets.GetTexture2D("HUD1440p"));
            resolutionSelect.AddOption("4K", Assets.GetTexture2D("HUD4K"));
            SelectableImage resolutionRight = new SelectableImage(Assets.GetTexture2D("HUDArrowRightBase"), Assets.GetTexture2D("HUDArrowRightSelected"), null, new Vector2(RIGHT_ARROW, currentYCoord), scale: 0.02f);
            resolutionRight.OnClick = () =>
            {
                resolutionSelect.Next();
            };
            SelectableImage resolutionLeft = new SelectableImage(Assets.GetTexture2D("HUDArrowLeftBase"), Assets.GetTexture2D("HUDArrowLeftSelected"), null, new Vector2(LEFT_ARROW, currentYCoord), scale: 0.02f);
            resolutionLeft.OnClick = () =>
            {
                resolutionSelect.Previous();
            };

            resolutionRight.HoverSoundEffectName = "MenuHover";
            resolutionRight.SelectSoundEffectName = "MenuSelect";
            resolutionLeft.HoverSoundEffectName = "MenuHover";
            resolutionLeft.SelectSoundEffectName = "MenuSelect";


            Image frameLimiterLabel = new Image(Assets.GetTexture2D("HUDFPSLimitLabel"), null, new Vector2(LABEL_POSITION, NextPosition()), scale: 0.25f);
            frameLimitSelect = new MultiSelectionImage(null, new Vector2(SELECTION_POSITION, NextPosition()), scale: 0.25f);
            frameLimitSelect.AddOption("30", Assets.GetTexture2D("HUD30"));
            frameLimitSelect.AddOption("60", Assets.GetTexture2D("HUD60"));
            frameLimitSelect.AddOption("120", Assets.GetTexture2D("HUD120"));
            frameLimitSelect.AddOption("Unlimited", Assets.GetTexture2D("HUDUnlimited"));

            SelectableImage fpsRight = new SelectableImage(Assets.GetTexture2D("HUDArrowRightBase"), Assets.GetTexture2D("HUDArrowRightSelected"), null, new Vector2(RIGHT_ARROW, currentYCoord), scale: 0.02f);
            fpsRight.OnClick = () =>
            {
                frameLimitSelect.Next();
            };
            SelectableImage fpsLeft = new SelectableImage(Assets.GetTexture2D("HUDArrowLeftBase"), Assets.GetTexture2D("HUDArrowLeftSelected"), null, new Vector2(LEFT_ARROW, currentYCoord), scale: 0.02f);
            fpsLeft.OnClick = () =>
            {
                frameLimitSelect.Previous();
            };

            fpsRight.HoverSoundEffectName = "MenuHover";
            fpsRight.SelectSoundEffectName = "MenuSelect";
            fpsLeft.HoverSoundEffectName = "MenuHover";
            fpsLeft.SelectSoundEffectName = "MenuSelect";

            Image vsyncLabel = new Image(Assets.GetTexture2D("HUDVsyncLabel"), null, new Vector2(LABEL_POSITION, NextPosition()), scale: 0.25f);
            vsyncSelect = new MultiSelectionImage(null, new Vector2(SELECTION_POSITION, NextPosition()), scale: 0.25f);
            vsyncSelect.AddOption("On", Assets.GetTexture2D("HUDOn"));
            vsyncSelect.AddOption("Off", Assets.GetTexture2D("HUDOff"));

            SelectableImage vsyncRight = new SelectableImage(Assets.GetTexture2D("HUDArrowRightBase"), Assets.GetTexture2D("HUDArrowRightSelected"), null, new Vector2(RIGHT_ARROW, currentYCoord), scale: 0.02f);
            vsyncRight.OnClick = () =>
            {
                vsyncSelect.Next();
            };
            SelectableImage vsyncLeft = new SelectableImage(Assets.GetTexture2D("HUDArrowLeftBase"), Assets.GetTexture2D("HUDArrowLeftSelected"), null, new Vector2(LEFT_ARROW, currentYCoord), scale: 0.02f);
            vsyncLeft.OnClick = () =>
            {
                vsyncSelect.Previous();
            };

            vsyncRight.HoverSoundEffectName = "MenuHover";
            vsyncRight.SelectSoundEffectName = "MenuSelect";
            vsyncLeft.HoverSoundEffectName = "MenuHover";
            vsyncLeft.SelectSoundEffectName = "MenuSelect";

            Image windowModeLabel = new Image(Assets.GetTexture2D("HUDWindowModeLabel"), null, new Vector2(LABEL_POSITION, NextPosition()), scale: 0.25f);
            windowModeSelect = new MultiSelectionImage(null, new Vector2(SELECTION_POSITION, NextPosition()), scale: 0.25f);
            windowModeSelect.AddOption("Fullscreen", Assets.GetTexture2D("HUDFullscreen"));
            windowModeSelect.AddOption("Windowed", Assets.GetTexture2D("HUDWindowed"));

            SelectableImage windowModeRight = new SelectableImage(Assets.GetTexture2D("HUDArrowRightBase"), Assets.GetTexture2D("HUDArrowRightSelected"), null, new Vector2(RIGHT_ARROW, currentYCoord), scale: 0.02f);
            windowModeRight.OnClick = () =>
            {
                windowModeSelect.Next();
            };
            SelectableImage windowModeLeft = new SelectableImage(Assets.GetTexture2D("HUDArrowLeftBase"), Assets.GetTexture2D("HUDArrowLeftSelected"), null, new Vector2(LEFT_ARROW, currentYCoord), scale: 0.02f);
            windowModeLeft.OnClick = () =>
            {
                windowModeSelect.Previous();
            };

            windowModeRight.HoverSoundEffectName = "MenuHover";
            windowModeRight.SelectSoundEffectName = "MenuSelect";
            windowModeLeft.HoverSoundEffectName = "MenuHover";
            windowModeLeft.SelectSoundEffectName = "MenuSelect";

            NextPosition();
            SelectableImage cancel = new SelectableImage(Assets.GetTexture2D("HUDCancelBase"), Assets.GetTexture2D("HUDCancelSelected"), null, new Vector2(LABEL_POSITION, NextPosition()), scale: 0.25f);
            cancel.HoverSoundEffectName = "MenuHover";
            cancel.SelectSoundEffectName = "MenuSelect";

            cancel.OnClick = () =>
            {
                SceneManager.StartScene("Settings");
            };

            SelectableImage apply = new SelectableImage(Assets.GetTexture2D("HUDApplyBase"), Assets.GetTexture2D("HUDApplySelected"), null, new Vector2(LABEL_POSITION * 2, currentYCoord), scale: 0.25f);
            apply.HoverSoundEffectName = "MenuHover";
            apply.SelectSoundEffectName = "MenuSelect";

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

            foreach (Camera camera in Cameras)
            {
                camera.Initialize();
            }

            SceneManager.OnResolutionChanged();
        }

        private int NextPosition()
        {
            currentYCoord += 5;
            return currentYCoord;
        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            PlatformerGame.Paused = true;
            SetCurrentVideoSettings();
        }

        public override void OnFinished()
        {

        }
    }
}
