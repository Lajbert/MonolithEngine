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

        public Level2Scene(LDTKMap world, SpriteFont spriteFont) : base("Level2", useLoadingScreen: true)
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

            Vector2 heroPosition = Vector2.Zero;
            List<(Vector2, Direction)> movingPlatforms = new List<(Vector2, Direction)>();

            foreach (EntityInstance entity in world.ParseLevel(this, "Level_2"))
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                if (entity.Identifier.Equals("Hero"))
                {
                    heroPosition = position;
                }
                else if (entity.Identifier.Equals("Coin"))
                {
                    bool hasGravity = true;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {
                        if (field.Identifier == "hasGravity")
                        {
                            hasGravity = field.Value;
                        }
                    }
                    Coin c = new Coin(this, position);
                    c.HasGravity = hasGravity;
                }
                else if (entity.Identifier.Equals("MovingPlatform"))
                {
                    new MovingPlatform(this, position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("Spring"))
                {
                    int power = -1;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {
                        if (field.Identifier == "power")
                        {
                            power = (int)field.Value;
                        }
                    }
                    Spring spring = new Spring(this, position, power);
                }
                else if (entity.Identifier.Equals("EnemyCarrot"))
                {
                    int speed = -1;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "speed")
                        {
                            speed = (int)field.Value;
                        }
                    }
                    Carrot carrot = new Carrot(this, position, Direction.EAST);
                }
                else if (entity.Identifier.Equals("Box"))
                {
                    new Box(this, position);
                }
                else if (entity.Identifier.Equals("Ladder"))
                {
                    new Ladder(this, position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("MovingPlatformTurn"))
                {
                    Direction dir = default;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {
                        if (field.Identifier == "Direction")
                        {
                            dir = Enum.Parse(typeof(Direction), field.Value);
                        }
                    }
                    movingPlatforms.Add((position, dir));
                }
                else if (entity.Identifier.Equals("SlideWall"))
                {
                    new SlideWall(this, position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("EnemyTrunk"))
                {
                    new Trunk(this, position, Direction.WEST);
                }
                else if (entity.Identifier.Equals("EnemySpikedTurtle"))
                {
                    new SpikedTurtle(this, position, Direction.WEST);
                }
                else if (entity.Identifier.Equals("Spikes"))
                {
                    Direction dir = default;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {
                        if (field.Identifier == "Direction")
                        {
                            dir = Enum.Parse(typeof(Direction), field.Value);
                        }
                    }
                    new Spikes(this, position, (int)entity.Width, dir);
                }
                else if (entity.Identifier.Equals("RespawnPoint"))
                {
                    new RespawnPoint(this, 256, 256, position);
                }
                else if (entity.Identifier.Equals("NextLevelTrigger"))
                {
                    //new NextLevelTrigger(this, position);
                }
            }

            hero = new Hero(this, heroPosition);
            foreach ((Vector2, Direction) prop in movingPlatforms)
            {
                new MovingPlatformTurner(this, prop.Item1, prop.Item2);
            }

        }

        public override void OnEnd()
        {
            AudioEngine.Pause("Level1Music");
        }

        public override void OnStart()
        {
            Camera.TrackTarget(hero, true);
            ForestPlatformerGame.Paused = false;
            ForestPlatformerGame.WasGameStarted = true;
            AudioEngine.Play("Level1Music");
        }

        public override void OnFinished()
        {
            SceneManager.StartScene("PauseMenu");
        }
    }
}
