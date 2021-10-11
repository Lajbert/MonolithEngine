using MonolithEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestPlatformerExample
{
    class MovingPlatform : PhysicalEntity
    {
        private static readonly float SPEED = 0.2f;

        private float speedX = SPEED;
        //private float speedX = 0;
        private float speedY = 0;

        private int directionX = -1;
        private int directionY = -1;

        public MovingPlatform(AbstractScene scene, Vector2 startPosition, int width, int height) : base(scene.LayerManager.EntityLayer, null, startPosition)
        {
            CheckGridCollisions = false;
            HasGravity = false;
            Active = true;
            HorizontalFriction = 0f;
            VerticalFriction = 0f;
            BumpFriction = 0f;
            Transform.VelocityX = speedX * directionX;
            Transform.VelocityY = speedY * directionY;
            //AddComponent(new Sprite(SpriteUtil.LoadTexture("ForestAssets/Tiles/forest-tileset"), new Rectangle(304, 288, Config.GRID, Config.GRID)));
            //new Sprite(this, Assets.GetTexture("ForestTileset"), new Rectangle(304, 288, Config.GRID, Config.GRID));
            AddComponent(new BoxCollisionComponent(this, width, height));
            TileGroup tg = new TileGroup();
            Texture2D tileSet = Assets.GetTexture("ForestTileset");
            Color[] data = new Color[Config.GRID * Config.GRID];
            tileSet.GetData(0, new Rectangle(304, 288, Config.GRID, Config.GRID), data, 0, data.Length);
            for (int i = 0; i < width; i += Config.GRID)
            {
                for (int j = 0; j < height; j += Config.GRID)
                {
                    tg.AddColorData(data, new Vector2(i, j));
                }
            }
            AddComponent(new Sprite(this, tg.GetTexture(), new Rectangle(0, 0, width, height)));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            AddCollisionAgainst(typeof(MovingPlatformTurner));
            AddTag("Mountable");
            AddTag("MovingPlatform");
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
                Transform.VelocityX = speedX * directionX;
                Transform.VelocityY = speedY * directionY;
            }
            
            base.OnCollisionStart(otherCollider);
        }

    }
}
