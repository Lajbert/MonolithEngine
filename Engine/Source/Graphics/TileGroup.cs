using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// A class to merge small, individual static textute into one spritemap.
    /// Extremely useful when a level is imported from a level editor where hundreds or
    /// thousands of individual texture are assigned to different positions of the map.
    /// Instead of drawing hundreds or thousands of separate textures, this class merges them
    /// into one texture and only one Draw() call will be called.
    /// Also useful when having any entity where the texture is repeated or constructed 
    /// from several other textures.
    /// </summary>
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

        /// <summary>
        /// Adds a new texture to a specific tile.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public void AddTile(Texture2D texture, Vector2 position, BlendMode blendMode = BlendMode.MERGE)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            AddColorData(data, position, blendMode);
        }

        /// <summary>
        /// Adds a new texture (as Color[]) to a specific tile.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
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

        /// <summary>
        /// When using BlendMode.MERGE, we want the new tile to be on top of the
        /// existing tile. It basically takes every non-transparent pixel from the new image
        /// and overwrites the existing pixel on the same position in the existing image.
        /// Similar to opening 2 images in a text editor, putting one 
        /// image on top of the other and saving the result as one single image.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Merges and returns the merge texture.
        /// </summary>
        /// <returns></returns>
        public Texture2D GetTexture()
        {
            if (texture == null)
            {
                Build();
            }

            return texture;
        }

        /// <summary>
        /// Creates the new merged texture from the individual small textures.
        /// </summary>
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

        /// <summary>
        /// Defines what we want to do when adding a new texture to an 
        /// existing position.
        /// </summary>
        public enum BlendMode
        {
            OVERWRITE,
            MERGE,
            NONE
        }

        public bool IsEmpty()
        {
            return width == 0 && height == 0;
        }
    }
}
