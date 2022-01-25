using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class Spikes : Entity
    {

        public Direction Direction;

        public Spikes(AbstractScene scene, Vector2 position, int length, Direction direction) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Direction = direction;
            AddTag("Spikes");
            TileGroup tg = new TileGroup(length, Config.GRID);
            Texture2D tileSet = Assets.GetTexture2D("ForestTileset");
            Sprite sprite = null;

            for (int i = 0; i < length; i += Config.GRID)
            {
                Rectangle rect;
                if (i == 0)
                {
                    rect = new Rectangle(240, 368, Config.GRID, Config.GRID);
                }
                else if (i == length - Config.GRID)
                {
                    rect = new Rectangle(272, 368, Config.GRID, Config.GRID);
                }
                else
                {
                    rect = new Rectangle(256, 368, Config.GRID, Config.GRID);
                }
                for (int j = 0; j < Config.GRID; j += Config.GRID)
                {
                    tg.AddTile(tileSet, new Vector2(i, j), rect);
                }
            }

            Rectangle boundingBox = new Rectangle(0, 0, length, Config.GRID);

            if (Direction == Direction.SOUTH || Direction == Direction.NORTH)
            {
                SpriteEffects flip = direction == Direction.SOUTH ? SpriteEffects.FlipVertically : SpriteEffects.None;
                sprite = new Sprite(this, new MonolithTexture(tg.GetTexture(), boundingBox, boundingBox));
                sprite.SpriteEffect = flip;
                AddComponent(new BoxCollisionComponent(this, length, Config.GRID));
            } 
            else if (Direction == Direction.WEST || Direction == Direction.EAST)
            {
                Vector2 offset = Vector2.Zero;
                float rotation;
                if (Direction == Direction.EAST)
                {
                    rotation = MathUtil.DegreesToRad(90);
                    offset = new Vector2(-Config.GRID, 0);
                }
                else 
                {
                    rotation = MathUtil.DegreesToRad(-90);
                    offset = new Vector2(0, -length);
                }
                sprite = new Sprite(this, new MonolithTexture(tg.GetTexture(), boundingBox, boundingBox), rotation: rotation, drawOffset: offset);
                AddComponent(new BoxCollisionComponent(this, Config.GRID, length));
            } 
            else
            {
                throw new Exception("Wrong spikes orientation");
            }
            
            AddComponent(sprite);
            
#if DEBUG
            (GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            DEBUG_SHOW_PIVOT = true;
#endif
            DrawPriority = 1;
        }
    }
}
