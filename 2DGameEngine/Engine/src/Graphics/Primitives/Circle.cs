using GameEngine2D.Entities;
using GameEngine2D.src.Layer;
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
        public Circle(GraphicsLayer layer, Entity parent, Vector2 center, int radius, Color color) : base(layer, parent, center, null)
        {
            sprite = CreateCircle(radius, color);
            this.color = color;
            this.center = center;
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, center, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        Texture2D CreateCircle(int radius, Color color)
        {
            Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, radius, radius);
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
