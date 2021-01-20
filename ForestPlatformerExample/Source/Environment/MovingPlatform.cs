using GameEngine2D;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class MovingPlatform : ControllableEntity
    {
        private float speed = Config.CHARACTER_SPEED;
        private float currentPos = 0f;

        private int direction = 1;

        List<Entity> platformElements = new List<Entity>();

        public MovingPlatform(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position)
        {

            for (int i = 0; i < 5; i++)
            {
                Entity e = new Entity(LayerManager.Instance.EntityLayer, this, new Vector2(0 + (i * Config.GRID), 0), SpriteUtil.CreateRectangle(Config.GRID, Color.Red), true);
                //e.Active = false;
                e.AddTag("MovingPlatform");
                platformElements.Add(e);

                e.Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);
            }
            SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));
            Active = true;
        }

        public override void Update(GameTime gameTime)
        {

            if (Math.Abs(StartPosition.X - Position.X) > 50)
            {
                direction *= -1;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            currentPos += speed;
            //Logger.Log("X: " + Position.X);
            float sin = (float)Math.Sin(currentPos) * elapsedTime;
            //Logger.Log("SIN: " + sin);
            foreach (Entity e in platformElements)
            {
                e.HasCollision = false;
            }
            //Direction.X += sin * elapsedTime;
            Velocity.X += speed * elapsedTime * direction;
            if (Velocity.X > 0.5f) Velocity.X = 0.5f;
            if (Velocity.X < -0.5f) Velocity.X = -0.5f;
            base.Update(gameTime);
            foreach (Entity e in platformElements)
            {
                e.Velocity = Velocity;
                e.HasCollision = true;
            }
        }
    }
}
