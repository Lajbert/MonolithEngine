using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    public class TileGroup
    {

        private Dictionary<Vector2, Color[]> tiles = new Dictionary<Vector2, Color[]>();

        private int width = 0;
        private int height = 0;

        public static GraphicsDevice GraphicsDevice;

        private Texture2D texture;

        public TileGroup()
        {
        }

        public void AddTile(Texture2D texture, Vector2 position)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            tiles.Add(position, data);
            width = Math.Max(width, (int)position.X + Config.GRID);
            height = Math.Max(height, (int)position.Y + Config.GRID);
        }

        public void AddColorData(Color[] data, Vector2 position, BlendMode blendMode = BlendMode.MERGE)
        {
            if (blendMode == BlendMode.MERGE)
            {
                if (tiles.ContainsKey(position)) {
                    Color[] result = MergeTile(tiles[position], data);
                    tiles[position] = result;
                }
                else
                {
                    tiles[position] = data;
                }

            }
            else if (blendMode == BlendMode.OVERWRITE)
            {
                tiles[position] = data;
            }
            else if (blendMode == BlendMode.NONE)
            {
                tiles.Add(position, data);
            }

            width = Math.Max(width, (int)position.X + Config.GRID);
            height = Math.Max(height, (int)position.Y + Config.GRID);
        }

        private Color[] MergeTile(Color[] data1, Color[] data2)
        {
            if (data1.Length != data2.Length)
            {
                throw new Exception("Can't merge different sized color arrays");
            }
            Color[] result = new Color[data1.Length];

            for (int i = 0; i < data1.Length; i++)
            {
                Color c = data1[i];
                if (data2[i].A == 255)
                {
                    c.R = data2[i].R;
                    c.G = data2[i].G;
                    c.B = data2[i].B;
                    c.A = data2[i].A;
                }
                result[i] = c;
            }

            return result;
        }

        public Texture2D GetTexture()
        {
            if (texture == null)
            {
                Build();
            }

            return texture;
        }

        private void Build()
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Incorrect tileset dimensions!");
            }

            texture = new Texture2D(GraphicsDevice, width, height);
            
            foreach(KeyValuePair<Vector2, Color[]> tile in tiles)
            {
                texture.SetData(0, new Rectangle((int)tile.Key.X, (int)tile.Key.Y, Config.GRID, Config.GRID), tile.Value, 0, tile.Value.Length);
            }
        }
        public enum BlendMode
        {
            OVERWRITE,
            MERGE,
            NONE
        }
    }
}
