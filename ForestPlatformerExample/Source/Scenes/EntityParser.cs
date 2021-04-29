using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies.Ghost;
using ForestPlatformerExample.Source.Entities.Enemies.IceCream;
using ForestPlatformerExample.Source.Entities.Enemies.Rock;
using ForestPlatformerExample.Source.Entities.Enemies.SpikedTurtle;
using ForestPlatformerExample.Source.Entities.Enemies.Trunk;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Entities.Traps;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Level;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Scenes
{
    class EntityParser
    {
        private LDTKMap world;

        private Hero hero;

        public EntityParser(LDTKMap world)
        {
            this.world = world;
        }

        public void LoadEntities(AbstractScene scene, string levelID)
        {
            Vector2 heroPosition = Vector2.Zero;
            List<(Vector2, Direction)> movingPlatforms = new List<(Vector2, Direction)>();

            foreach (EntityInstance entity in world.ParseLevel(scene, levelID))
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                Vector2 pivot = new Vector2((float)entity.Pivot[0], (float)entity.Pivot[1]);
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
                    Coin c = new Coin(scene, position);
                    c.HasGravity = hasGravity;
                }
                else if (entity.Identifier.Equals("MovingPlatform"))
                {
                    new MovingPlatform(scene, position, (int)entity.Width, (int)entity.Height);
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
                    Spring spring = new Spring(scene, position, power);
                }

                else if (entity.Identifier.Equals("Box"))
                {
                    new Box(scene, position);
                }
                else if (entity.Identifier.Equals("Ladder"))
                {
                    new Ladder(scene, position, (int)entity.Width, (int)entity.Height);
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
                    new SlideWall(scene, position, (int)entity.Width, (int)entity.Height);
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
                    float size = entity.Width > entity.Height ? entity.Width : entity.Height;
                    new Spikes(scene, position, (int)size, dir);
                }
                else if (entity.Identifier.Equals("RespawnPoint"))
                {
                    new RespawnPoint(scene, 256, 256, position);
                }
                else if (entity.Identifier.Equals("IceTrigger"))
                {
                    new IceTrigger(scene, (int)entity.Width, (int)entity.Height, position);
                }
                else if (entity.Identifier.Equals("NextLevelTrigger"))
                {
                    new NextLevelTrigger(scene, position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("EnemyTrunk"))
                {
                    new Trunk(scene, position, Direction.WEST);
                }
                else if (entity.Identifier.Equals("EnemySpikedTurtle"))
                {
                    new SpikedTurtle(scene, position, Direction.WEST);
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
                    Carrot carrot = new Carrot(scene, position, Direction.EAST);
                }
                else if (entity.Identifier.Equals("EnemyIceCream"))
                {
                    new IceCream(scene, position);
                }
                else if (entity.Identifier.Equals("Saw"))
                {
                    bool horizontal = true;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "HorizontalMovement")
                        {
                            horizontal = field.Value;
                        }
                    }
                    new Saw(scene, position, horizontal, pivot);
                }
                else if (entity.Identifier.Equals("SawPath"))
                {
                    new SawPath(scene, position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("Fan"))
                {
                    int forceFeildHeight = -1;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "forceFeildHeight")
                        {
                            forceFeildHeight = (int)field.Value;
                        }
                    }
                    new Fan(scene, position, forceFeildHeight);
                }
                else if (entity.Identifier.Equals("EnemyRock"))
                {
                    RockSize size = default;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "RockSize")
                        {
                            size = Enum.Parse(typeof(RockSize), field.Value);
                        }
                    }
                    new Rock(scene, position, size);
                }
                else if (entity.Identifier.Equals("EnemyGhost"))
                {
                    new Ghost(scene, position);
                }
                else if (entity.Identifier.Equals("GameFinishedTrophy"))
                {
                    new GameFinishTrophy(scene, position, pivot);
                }
                else if (entity.Identifier.Equals("PopupTextTrigger"))
                {
                    string textName = null;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "TextName")
                        {
                            textName = field.Value;
                        }
                    }
                    new PopupTrigger(scene, position, (int)entity.Width, (int)entity.Height, textName);
                }
            }

#if DEBUG
           /* PhysicalEntity collisionTest = new PhysicalEntity(scene.LayerManager.EntityLayer, null, new Vector2(17, 37) * Config.GRID)
            {
                HasGravity = false
            };
            collisionTest.SetSprite(Assets.CreateRectangle(64, Color.Yellow));
            collisionTest.AddTag("Mountable");
            //collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 32, 32, new Vector2(-16, -16)));
            collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 64, 64, Vector2.Zero));
            (collisionTest.GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;*/
#endif

            hero = new Hero(scene, heroPosition);
            foreach ((Vector2, Direction) prop in movingPlatforms)
            {
                new MovingPlatformTurner(scene, prop.Item1, prop.Item2);
            }

        }

        public Hero GetHero()
        {
            return hero;
        }
    }
}
