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

        public static GraphicsDevice GraphicsDevice;

        private RenderTarget2D renderTarget;

        private SpriteBatch spriteBatch;

        private Dictionary<Texture2D, List<TileGroupEntry>> textures = new Dictionary<Texture2D, List<TileGroupEntry>>();

        public TileGroup(int width, int height)
        {
            renderTarget = new RenderTarget2D(
                           GraphicsDevice,
                           width,
                           height,
                           false,
                           GraphicsDevice.PresentationParameters.BackBufferFormat,
                           DepthFormat.Depth24);
        }

        public void AddTile(Texture2D texture, Vector2 position, Rectangle sourceRectangle = default, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            List<TileGroupEntry> entries = new List<TileGroupEntry>();
            if (textures.ContainsKey(texture))
            {
                entries = textures[texture];
            }
            Rectangle drawRect = sourceRectangle == default ? new Rectangle(0, 0, texture.Width, texture.Height) : sourceRectangle;
            entries.Add(new TileGroupEntry(position, drawRect, spriteEffects));
            textures[texture] = entries;
        }
        public Texture2D GetTexture()
        {
            if (textures.Count == 0)
            {
                throw new Exception("Attempted to create empty TileGroup...");
            }
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            foreach (Texture2D texture in textures.Keys)
            {
                foreach (TileGroupEntry tge in textures[texture])
                {
                    spriteBatch.Draw(texture, tge.Position, tge.SourceRectangle, Color.White, 0f, Vector2.Zero, 1f, tge.SpriteEffects, 0);
                }
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        public bool IsEmpty()
        {
            return textures.Count == 0;
        }

        private struct TileGroupEntry
        {
            public Vector2 Position;
            public Rectangle SourceRectangle;
            public SpriteEffects SpriteEffects;

            public TileGroupEntry(Vector2 position, Rectangle sourceRectangle, SpriteEffects spriteEffects)
            {
                Position = position;
                SourceRectangle = sourceRectangle;
                SpriteEffects = spriteEffects;
            }
        }
    }
}
