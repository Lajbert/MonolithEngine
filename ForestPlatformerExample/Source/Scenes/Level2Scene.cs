using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies.SpikedTurtle;
using ForestPlatformerExample.Source.Entities.Enemies.Trunk;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Audio;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Source.Level;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
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
