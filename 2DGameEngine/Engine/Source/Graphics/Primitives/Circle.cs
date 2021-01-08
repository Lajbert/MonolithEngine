using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source;
using GameEngine2D.Source.Layer;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Graphics.Primitives
{
    public class Circle : Entity
    {
        private Vector2 center;
        private Color color;
        private float radius;
        private Vector2 offset;

        public Circle(Entity parent, Vector2 center, int radius, Color color) : base(RootContainer.Instance.EntityLayer, parent, center, null)
        {
            Sprite = SpriteUtil.CreateCircle(radius, color);
            this.color = color;
            this.center = center;
            this.radius = radius;
            this.offset = new Vector2(radius, radius) / 2;
        }

        protected override void DrawSprite(SpriteBatch spriteBatch, Vector2 position)
        {
            //base.DrawSprite(position);
            if (Visible)
            {
                spriteBatch.Draw(Sprite, position - offset, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
