using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.MyGame;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Level;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source
{
    class ForestPlatformerGame : MonolithGame
    {

        private Hero hero;

        public static int CoinCount = 0;

        private SpriteFont font;

        private UserInterface ui;

        protected override void Init()
        {
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void LoadGameContent()
        {

            LoadLevel();

            ui = new UserInterface();

            ui.AddUIElement(new Image("ForestAssets/UI/HUD-coin-count", new Vector2(30, 30), scale: 8));
            ui.AddUIElement(new SelectableImage("ForestAssets/UI/HUD-coin-count", "ForestAssets/UI/HUD-coin-count", new Vector2(30, 30), scale: 8));
            ui.AddUIElement(new TextField(font, () => CoinCount.ToString(), new Vector2(200, 5)));
        }

        private void LoadLevel()
        {

            MapSerializer mapSerializer = new LDTKJsonMapSerializer();
            LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/ForestPlatformerExample/Maps/level.json");
            foreach (EntityInstance entity in map.entities)
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                if (entity.Identifier.Equals("Hero"))
                {

                    hero = new Hero(position, font);
                    Camera.TrackTarget(hero, true);
                }
                else if (entity.Identifier.Equals("Coin"))
                {
                    new Coin(position);
                }
                else if (entity.Identifier.Equals("MovingPlatform"))
                {
                    new MovingPlatform(position, (int)entity.Width, (int)entity.Height);
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
                    Spring spring = new Spring(position, power);
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
                    Carrot carrot = new Carrot(position, Direction.EAST);
                }
                else if (entity.Identifier.Equals("Box"))
                {
                    Box box = new Box(position);
                }
                else if (entity.Identifier.Equals("Ladder"))
                {
                    new Ladder(position, (int)entity.Width, (int)entity.Height);
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
                    new MovingPlatformTurner(position, dir);
                }
                else if (entity.Identifier.Equals("SlideWall"))
                {
                    new SlideWall(position, (int)entity.Width, (int)entity.Height);
                }
            }

            PhysicalEntity collisionTest = new PhysicalEntity(LayerManager.Instance.EntityLayer, null, new Vector2(17, 37) * Config.GRID)
            {
                HasGravity = false
            };
            collisionTest.SetSprite(TextureUtil.CreateRectangle(64, Color.Yellow));
            collisionTest.AddTag("Mountable");
            //collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 32, 32, new Vector2(-16, -16)));
            collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 64, 64, Vector2.Zero));
            (collisionTest.GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

        }
    }
}
