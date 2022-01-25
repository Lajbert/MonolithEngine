using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Level1Scene : AbstractScene
    {

        private Hero hero;
        private LDTKMap world;
        private MobileButtonPanel controlButtons;
        private SelectableImage pauseButton;

        public Level1Scene(LDTKMap world) : base ("Level_1", useLoadingScreen: true)
        {
            this.world = world;
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
            Logger.Debug("Loading LEVEL 1: assets");
            LoadData();

            Logger.Debug("Loading LEVEL 1: adjusting camera");
            foreach (Camera cameara in Cameras)
            {
                cameara.Initialize();
                if (MonolithGame.Platform.IsMobile())
                {
                    cameara.Zoom += 0.5f;
                    cameara.Zoom *= 2;
                }
                else
                {
                    cameara.Zoom += 0.5f;
                }
            }
        }

        private void LoadData()
        {
            Logger.Debug("Loading LEVEL 1: UI");

            float coinImageScale = 2f;
            Image coinImage = new Image(Assets.GetTexture2D("HUDCointCount"), null, new Vector2(1, 2), scale: coinImageScale);
            UI.AddUIElement(coinImage);
            PNGFontRenderer coinCount = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), () => PlatformerGame.CoinCount.ToString(), new Vector2((coinImage.ImageTexture.Width + 5) * coinImageScale, 0), coinImage);
            coinCount.Scale = 4;

            Logger.Debug("Loading LEVEL 1: creating entity parser...");

            EntityParser parser = new EntityParser(world);

            Logger.Debug("Loading LEVEL 1: loading entities...");
            parser.LoadEntities(this, SceneName);
            parser.LoadIntGrid(this);

            hero = parser.GetHero();

            Image transparentBG = new Image(Assets.GetTexture2D("TransparentBG"), null, Vector2.Zero);

            GameButton continueButton = new GameButton(Assets.GetTexture2D("ContinueButton"), new Vector2(50, 50), 5);
            continueButton.OnClick = () => LayerManager.Paused = false;
            continueButton.OnClick += () => UI.RemoveUIElement(continueButton);
            continueButton.OnClick += () => UI.RemoveUIElement(transparentBG);
            continueButton.OnClick += AddButtons;

            pauseButton = new SelectableImage(Assets.GetTexture2D("PauseButton"), Assets.GetTexture2D("PauseButton"), null, new Vector2(92, 2), default, 2);
            pauseButton.OnClick += () => {
                if (!LayerManager.Paused)
                {
                    UI.AddUIElement(transparentBG);
                }
            };
            pauseButton.OnClick += () =>
            {
                if (!LayerManager.Paused)
                {
                    UI.AddUIElement(continueButton);
                }
            };
            pauseButton.OnClick += () =>
            {
                if (!LayerManager.Paused)
                {
                    RemoveButtons();
                }
            };
            pauseButton.OnClick += () => LayerManager.Paused = true;

            if (MonolithGame.Platform.IsMobile())
            {
                controlButtons = new MobileButtonPanel(hero);
            }

            AddButtons();

            world = null;
        }

        public override void OnEnd()
        {
            AudioEngine.Pause("Level1Music");
        }

        public void AddButtons()
        {
            UI.AddUIElement(pauseButton);

            if (MonolithGame.Platform.IsMobile())
            {
                foreach (SelectableImage button in controlButtons.GetButtons())
                {
                    UI.AddUIElement(button);
                }
            }
        }

        public void RemoveButtons()
        {
            UI.RemoveUIElement(pauseButton);

            if (MonolithGame.Platform.IsMobile())
            {
                foreach (SelectableImage button in controlButtons.GetButtons())
                {
                    UI.RemoveUIElement(button);
                }
            }
        }

        public override void OnStart()
        {
            Camera.TrackTarget(hero, true);
            PlatformerGame.Paused = false;
            PlatformerGame.WasGameStarted = true;
            AudioEngine.Play("Level1Music");
            PlatformerGame.CurrentScene = SceneName;
        }

        public override void OnFinished()
        {
            SceneManager.LoadScene("Level_2");
        }
    }
}
