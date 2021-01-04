using GameEngine2D.Engine.src.Layer;
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
    class Circle : Entity
    {


        private Vector2 center;
        private Color color;
        private float radius;
        private Vector2 offset;
        public bool Visible;

        public Circle(Entity parent, Vector2 center, int radius, Color color) : base(Scene.Instance.EntityLayer, parent, center, null)
        {
            Sprite = CreateCircle(radius, color);
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

        Texture2D CreateCircle(int radius, Color color)
        {
            Texture2D texture = new Texture2D(SpriteBatch.GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
