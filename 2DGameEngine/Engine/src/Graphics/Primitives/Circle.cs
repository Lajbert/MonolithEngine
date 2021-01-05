using GameEngine2D.Engine.src.Layer;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.src;
using GameEngine2D.src.Layer;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.src.Graphics.Primitives
{
    public class Circle : Entity
    {
        private Vector2 center;
        private Color color;
        private float radius;
        private Vector2 offset;
        public bool Visible;

        public Circle(Entity parent, Vector2 center, int radius, Color color) : base(Scene.Instance.EntityLayer, parent, center, null)
        {
            Sprite = SpriteUtil.CreateCircle(GraphicsDeviceManager, radius, color);
            this.color = color;
            this.center = center;
            this.radius = radius;
            this.offset = new Vector2(radius, radius) / 2;
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            if (Visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Sprite, position - offset, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                SpriteBatch.End();

            }
        }
    }
}
