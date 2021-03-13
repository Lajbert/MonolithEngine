using GameEngine2D.Engine.Source.Components;
using GameEngine2D.Engine.Source.Interfaces;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.GridCollision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Graphics
{
    public class Sprite : IComponent, IDrawableComponent
    {
        public bool UniquePerEntity { get; set; }
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public Vector2 DrawOffset;
        public Entity Owner;

        public Sprite(Entity owner, Texture2D texture, Rectangle? sourceRectangle = null, Vector2 drawOffset = default)
        {
            Texture = texture;
            if (sourceRectangle.HasValue)
            {
                SourceRectangle = sourceRectangle.Value;
            } 
            else
            {
                SourceRectangle = new Rectangle(0, 0, Config.GRID, Config.GRID);
            }
            
            UniquePerEntity = true;
            DrawOffset = drawOffset;
            Owner = owner;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Owner.Transform.Position + DrawOffset, SourceRectangle, Color.White, 0f, Owner.Pivot, 1f, SpriteEffects.None, Owner.Depth);
        }
    }
}
