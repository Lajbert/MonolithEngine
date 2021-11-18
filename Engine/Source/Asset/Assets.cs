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

        public static void LoadTexture(string name, string path, bool flipVertical = false, bool flipHorizontal = false, bool autoBoundingBox = false)
        {
            if (flipVertical || flipHorizontal)
            {
                textures.Add(name, new MonolithTexture(AssetUtil.FlipTexture(AssetUtil.LoadTexture(path), flipVertical, flipHorizontal), autoBoundingBox));
            }
            else
            {
                textures.Add(name, new MonolithTexture(AssetUtil.LoadTexture(path), autoBoundingBox: autoBoundingBox));
            }
        }

        public static void LoadTexture(string name, string path, Rectangle boundingBox, bool flipVertical = false, bool flipHorizontal = false)
        {
            if (flipVertical || flipHorizontal)
            {
                textures.Add(name, new MonolithTexture(AssetUtil.FlipTexture(AssetUtil.LoadTexture(path), flipVertical, flipHorizontal), boundingBox));
            }
            else
            {
                textures.Add(name, new MonolithTexture(AssetUtil.LoadTexture(path), boundingBox));
            }
        }

        public static void LoadAnimationTexture(string name, string path, int width = 0, int height = 0, int rows = 0, int columns = 0, int totalFrames = 0, bool flipVertical = false, bool flipHorizontal = false)
        {
            if (flipVertical || flipHorizontal)
            {
                textures.Add(name, new AnimationTexture(AssetUtil.FlipTexture(AssetUtil.LoadTexture(path), flipVertical, flipHorizontal), width, height, rows, columns, totalFrames));
            }
            else
            {
                textures.Add(name, new AnimationTexture(AssetUtil.LoadTexture(path), width, height, rows, columns, totalFrames));
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

        public static AnimationTexture GetAnimationTexture(string name)
        {
            return (AnimationTexture)textures[name];
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
