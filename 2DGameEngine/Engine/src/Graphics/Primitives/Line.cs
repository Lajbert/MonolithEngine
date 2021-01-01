using GameEngine2D.Entities;
using GameEngine2D.src.Layer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.src.Graphics.Primitives
{
    class Line : Entity
    {
        private Vector2 origin;
        //Vector2 origin = new Vector2(100f, 100f);
        private Vector2 scale;

        private Vector2 from;
        private Vector2 to;
        private Color color;
        private float thickness;

        private float angle;
        private float distance;

        public Line(GraphicsLayer layer, Entity parent, Vector2 from, Vector2 to, Color color, float thickness = 1f) : base(layer, parent, from, null)
        {
            this.from = from;
            this.to = to;
            this.thickness = thickness;
            this.color = color;
            sprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sprite.SetData(new[] { Color.White });
            distance = Vector2.Distance(from, to);
            angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public Line(GraphicsLayer layer, Entity parent, Vector2 from, float angle, float distance, Color color, float thickness = 1f) : base(layer, parent, from, null)
        {
            this.from = from;
            this.thickness = thickness;
            this.color = color;
            sprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sprite.SetData(new[] { Color.White });
            this.distance = distance;
            this.angle = (float)(Math.PI / 180) * angle;
        }

        private void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            float distance = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(point1, distance, angle, color, thickness);
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            DrawLine(from, distance, angle, color, thickness);
        }

        private void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            origin = new Vector2(0f, 0.5f);
            //Vector2 origin = new Vector2(100f, 100f);
            scale = new Vector2(length, thickness);
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
