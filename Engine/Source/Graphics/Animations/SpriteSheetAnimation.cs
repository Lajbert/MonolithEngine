﻿using Microsoft.Xna.Framework;
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
        private MonolithTexture texture;
        private int rows;
        private int columns;
        private int width;
        private int height;

        private int currentRow;
        private int currentColumn;

#if DEBUG
        public bool DEBUG_SPRITESHEET = false;
#endif

        private Dictionary<int, Rectangle> sourceRectangles = new Dictionary<int, Rectangle>();

        //private int frameSize;

        public SpriteSheetAnimation(Entity parent, MonolithTexture texture, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None, int frameSizeOverride = 0) : base(parent, 0, framerate, spriteEffect)
        {
            int frameSize;
            this.texture = texture;
            if (frameSizeOverride == 0)
            {
                frameSize = GetFrameSize();
            } else
            {
                frameSize = frameSizeOverride;
            }
            rows = texture.GetTexture2D().Height / frameSize;
            columns = texture.GetTexture2D().Width / frameSize;
            this.width = frameSize;
            this.height = frameSize;
            TotalFrames = GetFrameCount();
            SetupSourceRectangles();
        }

        private SpriteSheetAnimation(Entity parent, MonolithTexture texture, int rows, int columns, int totalFrames, int width, int height, int framerate, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, totalFrames, framerate, spriteEffect)
        {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            if (width == 0 || height == 0)
            {
                this.width = texture.GetTexture2D().Width / columns;
                this.height = texture.GetTexture2D().Height / rows;
            } else
            {
                this.width = width;
                this.height = height;
            }
        }

        public SpriteSheetAnimation(Entity parent, MonolithTexture texture, int frameWidth, int frameHeight, int framerate, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, 0, framerate, spriteEffect)
        {
            this.texture = texture;
            this.width = frameWidth;
            this.height = frameHeight;
            this.rows = texture.GetTexture2D().Height / frameHeight;
            this.columns = texture.GetTexture2D().Width / frameWidth;
            TotalFrames = GetFrameCount();
            SetupSourceRectangles();
        }

        private SpriteSheetAnimation Copy()
        {
            SpriteSheetAnimation newAnim = new SpriteSheetAnimation(Parent, null, rows, columns, 0, width, height, 1, SpriteEffect)
            {
                texture = texture
            };
            base.Copy(newAnim);
            //newAnim.SetupSourceRectangles();
            return newAnim;
        }

        private void SetupSourceRectangles()
        {
            for (int i = StartFrame; i <= EndFrame; i++)
            {
                currentRow = (int)((float)i / (float)columns);
                currentColumn = i % columns;
                sourceRectangles.Add(i, new Rectangle(width * currentColumn, height * currentRow, width, height));
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
            Origin = new Vector2(width / 2, height / 2);
            return texture.GetTexture2D();
        }

        private int GetFrameSize()
        {
            int longerSide = Math.Max(texture.GetTexture2D().Width, texture.GetTexture2D().Height);

            int biggestFrame = 0;

            for (int i = 1; i <= Math.Log(longerSide); i++)
            {
                int pow = (int)Math.Pow(2, i);
                if (texture.GetTexture2D().Width % pow == 0 && texture.GetTexture2D().Height % pow == 0)
                {
                    biggestFrame = pow;
                }
            }
            if (biggestFrame == 0)
            {
                throw new Exception("Can't determine frame size, the image dimensions are not the multiples of power of 2");
            }
            return biggestFrame;
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

        // Automatically determines the frame count in the sprite sheet.
        // Smart enough not to count empty frames at the end.
        private int GetFrameCount()
        {
            int frameCount = 0;
            Color[] allPixelData = texture.GetPixels();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Color[] data = AssetUtil.GetPixels(allPixelData, new Rectangle(j * width, i * height, width, height), texture.GetTexture2D().Width);
                    bool emptyFrameFound = true;
                    for (int c = 0; c < width * height; c++)
                    {
                        if (data[c].ToVector4() != Vector4.Zero)
                        {
                            emptyFrameFound = false;
                            break;
                        }
                    }
                    if (emptyFrameFound)
                    {
                        return frameCount;
                    }
                    frameCount++;
                }
            }
            return frameCount;
        }

        public override void Destroy()
        {
            texture = null;
        }
    }
}
