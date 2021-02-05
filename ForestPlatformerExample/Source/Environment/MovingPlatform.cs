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
    class MovingPlatform : PhysicalEntity
    {
        private float speed = Config.CHARACTER_SPEED;
        private float currentPos = 0f;

        private int direction = -1;
        
        private int travelDistance = 0;

        List<Entity> platformElements = new List<Entity>();

        private Texture2D texture = null;

        private Vector2 startPosition;

        public MovingPlatform(int travelDistance, Vector2 startPosition) : base(LayerManager.Instance.EntityLayer, null, startPosition)
        {
            this.travelDistance = travelDistance;
            this.startPosition = startPosition;
            HasGravity = false;
            SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));
            Active = true;
            texture = SpriteUtil.LoadTexture("ForestAssets/Tiles/forest-tileset");
        }

        public void AddPlatformElement(Vector2 position)
        {
            Entity e = new Entity(LayerManager.Instance.EntityLayer, this, startPosition - position, texture);
            e.BlocksMovement = false;
            //e.Active = false;
            e.AddTag("MovingPlatform");
            platformElements.Add(e);
            //e.SetSprite(texture);
            e.SourceRectangle = new Rectangle(304, 288, Config.GRID, Config.GRID);
            //e.Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);
        }

        public override void Update(GameTime gameTime)
        {

            if (Math.Abs(startPosition.X - Position.X) > travelDistance)
            {
                direction *= -1;
            }
            if (Position.X < startPosition.X)
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
                //e.HasCollision = false;
            }
            //Direction.X += sin * elapsedTime;
            Velocity.X += speed * elapsedTime * direction;
            if (Velocity.X > 0.5f) Velocity.X = 0.5f;
            if (Velocity.X < -0.5f) Velocity.X = -0.5f;
            base.Update(gameTime);
            foreach (Entity e in platformElements)
            {
                //e.Velocity = Velocity;
                //e.HasCollision = true;
            }
        }
    }
}
