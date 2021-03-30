﻿using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Scene.Transition;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Camera2D;
using MonolithEngine.Source.Level;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class Level1Scene : AbstractScene
    {

        private Hero hero;
        private SpriteFont font;
        private List<GameObject> objects;

        public Level1Scene(Camera camera, SpriteFont spriteFont) : base (camera, "level1")
        {
            font = spriteFont;
            objects = new List<GameObject>();
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
        }

        private void LoadData()
        {

            UI.AddUIElement(new Image("ForestAssets/UI/HUD-coin-count", new Vector2(30, 30), scale: 8));
            UI.AddUIElement(new SelectableImage("ForestAssets/UI/HUD-coin-count", "ForestAssets/UI/HUD-coin-count", new Vector2(30, 30), scale: 8));
            UI.AddUIElement(new TextField(font, () => ForestPlatformerGame.CoinCount.ToString(), new Vector2(200, 5)));

            MapSerializer mapSerializer = new LDTKJsonMapSerializer();
            LDTKMap map = mapSerializer.Deserialize(this, "D:/GameDev/MonoGame/2DGameEngine/ForestPlatformerExample/Maps/level.json");
            foreach (EntityInstance entity in map.entities)
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                if (entity.Identifier.Equals("Hero"))
                {

                    hero = new Hero(this, position, font);
                }
                else if (entity.Identifier.Equals("Coin"))
                {
                    objects.Add(new Coin(this, position));
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
                    objects.Add(spring);
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
                    objects.Add(carrot);
                }
                else if (entity.Identifier.Equals("Box"))
                {
                    objects.Add(new Box(this, position));
                }
                else if (entity.Identifier.Equals("Ladder"))
                {
                    objects.Add(new Ladder(this, position, (int)entity.Width, (int)entity.Height));
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
                    objects.Add(new MovingPlatformTurner(this, position, dir));
                }
                else if (entity.Identifier.Equals("SlideWall"))
                {
                    objects.Add(new SlideWall(this, position, (int)entity.Width, (int)entity.Height));
                }
            }

            PhysicalEntity collisionTest = new PhysicalEntity(LayerManager.EntityLayer, null, new Vector2(17, 37) * Config.GRID)
            {
                HasGravity = false
            };
            collisionTest.SetSprite(TextureUtil.CreateRectangle(64, Color.Yellow));
            collisionTest.AddTag("Mountable");
            //collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 32, 32, new Vector2(-16, -16)));
            collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 64, 64, Vector2.Zero));
            (collisionTest.GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

        }

        public override void OnEnd()
        {
            
        }

        public override void OnStart()
        {
            Camera.TrackTarget(hero, true);
        }

        public override void Unload()
        {
            foreach (GameObject entity in objects)
            {
                entity.Destroy();
            }
        }
    }
}
