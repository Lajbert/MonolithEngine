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

        public SpriteGroupAnimation(List<Texture2D> textures, Entity parent, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, textures.Count, framerate, spriteEffect)
        {
            this.Textures = textures;
        }

        protected override Texture2D GetTexture()
        {
            return Textures[CurrentFrame];
        }
    }
}
