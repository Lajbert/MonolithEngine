﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Animation class representing an animation where the frames are 
    /// drawn on separate image files.
    /// Each frame points to it's own texture and returns the same source
    /// rectangle (size of the frame).
    /// </summary>
    public class SpriteGroupAnimation : AbstractAnimation
    {
        // frames of the animation
        public List<Texture2D> Textures;

        public SpriteGroupAnimation(Entity parent, List<Texture2D> textures, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, textures.Count, framerate, spriteEffect)
        {
            this.Textures = textures;
        }

        public SpriteGroupAnimation Copy()
        {
            SpriteGroupAnimation newAnim = new SpriteGroupAnimation(Parent, null, 0, SpriteEffect)
            {
                Textures = Textures
            };
            base.Copy(newAnim);
            return newAnim;
        }

        public SpriteGroupAnimation CopyFlipped()
        {
            SpriteGroupAnimation newAnim = Copy();
            newAnim.Flip();
            return newAnim;
        }

        internal override Texture2D GetTexture()
        {
            Origin = new Vector2((float)Math.Floor((decimal)Textures[CurrentFrame].Width / 2), (float)Math.Floor((decimal)Textures[CurrentFrame].Height / 2));
            SourceRectangle = new Rectangle(0, 0, Textures[CurrentFrame].Width, Textures[CurrentFrame].Height);
            return Textures[CurrentFrame];
        }

        public override void Destroy()
        {
            Textures = null;
        }
    }
}
