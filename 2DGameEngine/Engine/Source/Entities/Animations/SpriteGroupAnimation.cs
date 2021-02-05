using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities.Animation
{
    public class SpriteGroupAnimation : AbstractAnimation
    {
        public List<Texture2D> Textures;

        public SpriteGroupAnimation(Entity parent, List<string> texturepaths, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, texturepaths.Count, framerate, spriteEffect)
        {
            if (texturepaths != null)
            {
                this.Textures = TextureCache.GetTextures(texturepaths);
            }
        }

        public SpriteGroupAnimation Copy()
        {
            SpriteGroupAnimation newAnim = new SpriteGroupAnimation(Parent, null, 0, SpriteEffect);
            newAnim.Textures = Textures;
            base.Copy(newAnim);
            return newAnim;
        }

        public SpriteGroupAnimation CopyFlipped()
        {
            SpriteGroupAnimation newAnim = Copy();
            newAnim.Flip();
            return newAnim;
        }

        protected override Texture2D GetTexture()
        {
            Pivot = new Vector2((float)Math.Floor((decimal)Textures[CurrentFrame].Width / 2), (float)Math.Floor((decimal)Textures[CurrentFrame].Height / 2));
            SourceRectangle = new Rectangle(0, 0, Textures[CurrentFrame].Width, Textures[CurrentFrame].Height);
            return Textures[CurrentFrame];
        }

        public override void Destroy()
        {
            Textures = null;
        }
    }
}
