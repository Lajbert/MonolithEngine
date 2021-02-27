using GameEngine2D;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Trigger;
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
        private float speedX = 0.1f;
        //private float speedX = 0;
        private float speedY = 0;

        public int DirectionX = -1;
        public int DirectionY = -1;

        public MovingPlatform(Vector2 startPosition, int width, int height) : base(LayerManager.Instance.EntityLayer, null, startPosition)
        {
            HasGravity = false;
            Active = true;
            //AddComponent(new Sprite(SpriteUtil.LoadTexture("ForestAssets/Tiles/forest-tileset"), new Rectangle(304, 288, Config.GRID, Config.GRID)));
            AddComponent(new BoxCollisionComponent(this, width, height));
            (GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            AddCollisionAgainst("PlatformTurner");
            AddTag("MovingPlatform");
        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            DirectionX *= -1;
            base.OnCollisionStart(otherCollider);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Velocity.X = speedX * elapsedTime * DirectionX;
            Velocity.Y = speedY * elapsedTime * DirectionY;
            //if (Velocity.X > 0.5f) Velocity.X = 0.5f;
            //if (Velocity.X < -0.5f) Velocity.X = -0.5f;
            base.Update(gameTime);
        }
    }
}
