using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities.Animation
{
    public class AnimatedSpriteGroup : AbstractAnimation
    {
        public List<Texture2D> Textures { get; set; }

        public AnimatedSpriteGroup(List<Texture2D> textures, Entity parent, SpriteBatch spriteBatch,  int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(spriteBatch, parent, textures.Count, framerate, spriteEffect)
        {
            this.Textures = textures;
        }

        protected override Texture2D GetTexture()
        {
            return Textures[CurrentFrame];
        }
    }
}
