using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Level2Scene : AbstractScene
    {
        private Hero hero;
        private SpriteFont font;
        private LDTKMap world;

        public Level2Scene(LDTKMap world, SpriteFont spriteFont) : base("Level_2", useLoadingScreen: true)
        {
            font = spriteFont;
            this.world = world;
            BackgroundColor = Color.LightBlue;
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
            LoadData();
            foreach (Camera cameara in Cameras)
            {
                cameara.Initialize();
                if (PlatformerGame.ANDROID)
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

            UI.AddUIElement(new Image(Assets.GetTexture("HUDCointCount"), new Vector2(5, 5), scale: 2));
            UI.AddUIElement(new TextField(font, () => PlatformerGame.CoinCount.ToString(), new Vector2(50, 5), scale: 0.2f));

            EntityParser parser = new EntityParser(world);

            parser.LoadEntities(this, SceneName);
            parser.LoadIntGrid(this);

            hero = parser.GetHero();

            if (PlatformerGame.ANDROID)
            {
                MobileButtonPanel controlButtons = new MobileButtonPanel(hero);
                foreach (SelectableImage button in controlButtons.GetButtons())
                {
                    UI.AddUIElement(button);
                }
            }

        }

        public override void OnEnd()
        {
            AudioEngine.Pause("Level2Music");
        }

        public override void OnStart()
        {
            foreach (Camera camera in Cameras)
            {
                camera.TrackTarget(hero, true);
            }
            PlatformerGame.Paused = false;
            PlatformerGame.WasGameStarted = true;
            AudioEngine.Play("Level2Music");
            PlatformerGame.CurrentScene = SceneName;
        }

        public override void OnFinished()
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}
