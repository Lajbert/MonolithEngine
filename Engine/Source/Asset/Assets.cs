using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Simple helper class to load, cache and access various assets.
    /// </summary>
    public class Assets
    {
        private static Dictionary<string, MonolithTexture> textures = new Dictionary<string, MonolithTexture>();

        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        public static void LoadTexture(string name, string path, bool flipVertical = false, bool flipHorizontal = false, bool cachePixels = false)
        {
            if (flipVertical || flipHorizontal)
            {
                textures.Add(name, new MonolithTexture(AssetUtil.FlipTexture(AssetUtil.LoadTexture(path).GetTexture2D(), flipVertical, flipHorizontal), cachePixels: cachePixels));
            }
            else
            {
                textures.Add(name, AssetUtil.LoadTexture(path, cachePixels));
            }
        }

        public static Texture2D GetTexture2D(string name)
        {
            return textures[name].GetTexture2D();
        }

        public static MonolithTexture GetTexture(string name)
        {
            return textures[name];
        }

        public static Texture2D LoadAndGetTexture2D(string name, string path)
        {
            LoadTexture(name, path);
            return textures[name].GetTexture2D();
        }

        public static Texture2D CreateRectangle(int size, Color color)
        {
            return AssetUtil.CreateRectangle(size, color);
        }

        public static Texture2D CreateRectangle(int width, int height, Color color)
        {
            return AssetUtil.CreateRectangle(width, height, color);
        }

        public static Texture2D CreateCircle(int diameter, Color color, bool filled = false)
        {
            return AssetUtil.CreateCircle(diameter, color, filled);
        }

        public static void AddFont(string name, SpriteFont spriteFont)
        {
            fonts.Add(name, spriteFont);
        }

        public static SpriteFont GetFont(string name)
        {
            return fonts[name];
        }
    }
}
