using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
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
        private static readonly float SPEED = 0.5f;

        private float speedX = SPEED;
        //private float speedX = 0;
        private float speedY = 0;

        private int directionX = -1;
        private int directionY = -1;

        public MovingPlatform(Vector2 startPosition, int width, int height) : base(LayerManager.Instance.EntityLayer, null, startPosition)
        {
            HasGravity = false;
            Active = true;
            Friction = 0f;
            BumpFriction = 0f;
            VelocityX = speedX * directionX;
            VelocityY = speedY * directionY;
            //AddComponent(new Sprite(SpriteUtil.LoadTexture("ForestAssets/Tiles/forest-tileset"), new Rectangle(304, 288, Config.GRID, Config.GRID)));
            Sprite s = new Sprite(this, TextureUtil.LoadTexture("ForestAssets/Tiles/forest-tileset"), new Rectangle(304, 288, Config.GRID, Config.GRID));
            AddComponent(new BoxCollisionComponent(this, width, height));
            TileGroup tg = new TileGroup();
            Texture2D tileSet = TextureUtil.LoadTexture("ForestAssets/Tiles/forest-tileset");
            Color[] data = new Color[Config.GRID * Config.GRID];
            tileSet.GetData<Color>(0, new Rectangle(304, 288, Config.GRID, Config.GRID), data, 0, data.Length);
            for (int i = 0; i < width; i += Config.GRID)
            {
                for (int j = 0; j < height; j += Config.GRID)
                {
                    tg.AddColorData(data, new Vector2(i, j));
                }
            }
            AddComponent(new Sprite(this, tg.GetTexture(), new Rectangle(0, 0, width, height)));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            AddCollisionAgainst("PlatformTurner");
            AddTag("Mountable");
        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            if (otherCollider is MovingPlatformTurner)
            {
                MovingPlatformTurner turner = otherCollider as MovingPlatformTurner;
                if (turner.TurnDirection == Direction.WEST)
                {
                    directionX = -1;
                    speedY = 0;
                    speedX = SPEED;
                } else if (turner.TurnDirection == Direction.EAST)
                {
                    directionX = 1;
                    speedY = 0;
                    speedX = SPEED;
                }
                else if(turner.TurnDirection == Direction.NORTH)
                {
                    directionY = -1;
                    speedX = 0;
                    speedY = SPEED;
                }
                else if(turner.TurnDirection == Direction.SOUTH)
                {
                    directionY = 1;
                    speedX = 0;
                    speedY = SPEED;
                }
                VelocityX = speedX * directionX;
                VelocityY = speedY * directionY;
            }
            
            base.OnCollisionStart(otherCollider);
        }

    }
}
