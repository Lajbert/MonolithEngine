using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Animation class representing an animation where the frames are 
    /// drawn on one spritesheet image file.
    /// Each frame points to the same texture with different source
    /// rectangle (the position of the current frame in the spritesheet).
    /// It automatically creates the texture and source rectangles based on
    /// the sprite sheet with ignoring empty frames.
    /// </summary>
    public class SpriteSheetAnimation : AbstractAnimation
    {
        private AnimationTexture texture;

        private int currentRow;
        private int currentColumn;

#if DEBUG
        public bool DEBUG_SPRITESHEET = false;
#endif

        private Dictionary<int, Rectangle> sourceRectangles = new Dictionary<int, Rectangle>();

        public SpriteSheetAnimation(Entity parent, AnimationTexture texture, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, 0, framerate, spriteEffect)
        {
            this.texture = texture;
            TotalFrames = texture.FrameCount;
            SetupSourceRectangles();
        }

        private SpriteSheetAnimation Copy()
        {
            SpriteSheetAnimation newAnim = new SpriteSheetAnimation(Parent, texture, Framerate, SpriteEffect)
            {
                texture = texture
            };
            base.Copy(newAnim);
            return newAnim;
        }

        private void SetupSourceRectangles()
        {
            for (int i = StartFrame; i <= EndFrame; i++)
            {
                currentRow = (int)((float)i / (float)texture.Columns);
                currentColumn = i % texture.Columns;
                sourceRectangles.Add(i, new Rectangle(texture.Width * currentColumn, texture.Height * currentRow, texture.Width, texture.Height));
            }
        }

        public SpriteSheetAnimation CopyFlipped()
        {
            SpriteSheetAnimation newAnim = Copy();
            newAnim.Flip();
            newAnim.SetupSourceRectangles();
            return newAnim;
        }

        internal override Texture2D GetTexture()
        {
            SourceRectangle = sourceRectangles[CurrentFrame];
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            return texture.GetTexture2D();
        }

#if DEBUG
        public override void Play(SpriteBatch spriteBatch)
        {
            if (DEBUG_SPRITESHEET)
            {
                spriteBatch.Draw(texture.GetTexture2D(), Parent.Transform.Position, Color.White);
            }
            else
            {
                base.Play(spriteBatch);
            }
        }
#endif

        public override void Destroy()
        {
            texture = null;
        }
    }
}
