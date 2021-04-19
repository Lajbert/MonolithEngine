using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Util;
using System;

namespace ForestPlatformerExample.Source.Environment
{
    class Spikes : Entity
    {

        public Direction Direction;

        public Spikes(AbstractScene scene, Vector2 position, int length, Direction direction) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Direction = direction;
            AddTag("Spikes");
            TileGroup tg = new TileGroup();
            Texture2D tileSet = Assets.GetTexture("ForestTileset");
            Color[] data = new Color[Config.GRID * Config.GRID];
            Sprite sprite = null;

            for (int i = 0; i < length; i += Config.GRID)
            {
                if (i == 0)
                {
                    tileSet.GetData(0, new Rectangle(240, 368, Config.GRID, Config.GRID), data, 0, data.Length);
                }
                else if (i == length - Config.GRID)
                {
                    data = new Color[Config.GRID * Config.GRID];
                    tileSet.GetData(0, new Rectangle(272, 368, Config.GRID, Config.GRID), data, 0, data.Length);
                }
                else
                {
                    data = new Color[Config.GRID * Config.GRID];
                    tileSet.GetData(0, new Rectangle(256, 368, Config.GRID, Config.GRID), data, 0, data.Length);
                }
                for (int j = 0; j < Config.GRID; j += Config.GRID)
                {
                    tg.AddColorData(data, new Vector2(i, j));
                }
            }

            if (Direction == Direction.SOUTH || Direction == Direction.NORTH)
            {

                sprite = new Sprite(this, tg.GetTexture(), new Rectangle(0, 0, length, Config.GRID), flipVertical: direction == Direction.SOUTH);
                AddComponent(new BoxCollisionComponent(this, length, Config.GRID));
            } 
            else if (Direction == Direction.WEST || Direction == Direction.EAST)
            {
                Vector2 offset = Vector2.Zero;
                float rotation;
                if (Direction == Direction.EAST)
                {
                    rotation = MathUtil.DegreesToRad(90);
                    offset = new Vector2(Config.GRID, 0);
                }
                else 
                {
                    rotation = MathUtil.DegreesToRad(-90);
                    offset = new Vector2(0, length);
                }
                sprite = new Sprite(this, tg.GetTexture(), new Rectangle(0, 0, length, Config.GRID), rotation: rotation, drawOffset: offset);
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
