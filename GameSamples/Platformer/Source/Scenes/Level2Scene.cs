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
            Camera.SetScale();
            Camera.Zoom += 0.5f;
        }

        private void LoadData()
        {

            UI.AddUIElement(new Image(Assets.GetTexture("HUDCointCount"), new Vector2(5, 5), scale: 2));
            UI.AddUIElement(new TextField(font, () => ForestPlatformerGame.CoinCount.ToString(), new Vector2(50, 5), scale: 0.2f));

            EntityParser parser = new EntityParser(world);

            parser.LoadEntities(this, SceneName);

            hero = parser.GetHero();

        }

        public override void OnEnd()
        {
            AudioEngine.Pause("Level2Music");
        }

        public override void OnStart()
        {
            Camera.TrackTarget(hero, true);
            ForestPlatformerGame.Paused = false;
            ForestPlatformerGame.WasGameStarted = true;
            AudioEngine.Play("Level2Music");
            ForestPlatformerGame.CurrentScene = SceneName;
        }

        public override void OnFinished()
        {
            SceneManager.StartScene("EndScene");
        }
    }
}
