using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Level1Scene : AbstractScene
    {

        private Hero hero;
        private SpriteFont font;
        private LDTKMap world;

        public Level1Scene(LDTKMap world, SpriteFont spriteFont) : base ("Level_1", useLoadingScreen: true)
        {
            font = spriteFont;
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
            Camera.Initialize();
            if (PlatformerGame.ANDROID)
            {
                Camera.Zoom += 0.5f;
                Camera.Zoom *= 2;
            }
            else
            {
                Camera.Zoom += 0.5f;
            }

        }

        private void LoadData()
        {
            Logger.Debug("Loading LEVEL 1: UI");

            UI.AddUIElement(new Image(Assets.GetTexture("HUDCointCount"), new Vector2(5, 5), scale: 2));
            UI.AddUIElement(new TextField(font, () => PlatformerGame.CoinCount.ToString(), new Vector2(50, 5), scale: 2.5f));

            Logger.Debug("Loading LEVEL 1: creating entity parser...");

            EntityParser parser = new EntityParser(world);

            Logger.Debug("Loading LEVEL 1: loading entities...");
            parser.LoadEntities(this, SceneName);

            hero = parser.GetHero();

            if (PlatformerGame.ANDROID)
            {
                MobileButtonPanel controlButtons = new MobileButtonPanel(hero);
                foreach (SelectableImage button in controlButtons.GetButtons())
                {
                    UI.AddUIElement(button);
                }
            }

            world = null;
        }

        public override void OnEnd()
        {
            AudioEngine.Pause("Level1Music");
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
