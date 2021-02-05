using GameEngine2D.Engine.Source.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Graphics
{
    public class TextureCache
    {
        private static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        public static Texture2D GetTexture(string path)
        {
            if (!cache.ContainsKey(path))
            {
                cache[path] = SpriteUtil.LoadTexture(path);
            }

            return cache[path];
        }

        public static List<Texture2D> GetTextures(List<string> paths)
        {
            List<Texture2D> result = new List<Texture2D>();
            foreach (string path in paths)
            {
                if (!cache.ContainsKey(path))
                {
                    cache[path] = SpriteUtil.LoadTexture(path);
                }
                result.Add(cache[path]);
            }

            return result;
        }
    }
}
