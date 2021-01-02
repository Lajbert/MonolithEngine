using GameEngine2D.Entities;
using GameEngine2D.src.Layer;
using GameEngine2D.src.Util;
using GameEngine2D.Util;
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
        private Vector2 scale;

        public Vector2 from;
        public Vector2 to;
        private Color color;
        private float thickness;

        private float angleRad;
        private float length;

        public Line(GraphicsLayer layer, Entity parent, Vector2 from, Vector2 to, Color color, float thickness = 1f) : base(layer, parent, from, null)
        {
            this.from = from;
            this.to = to;
            this.thickness = thickness;
            this.color = color;
            sprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sprite.SetData(new[] { Color.White });
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.AngleFromVectors(from, to);
            origin = new Vector2(0f, 0.5f);
            scale = new Vector2(length, thickness);
        }

        public Line(GraphicsLayer layer, Entity parent, Vector2 from, float angleRad, float length, Color color, float thickness = 1f) : base(layer, parent, from, null)
        {
            this.from = from;
            this.thickness = thickness;
            this.color = color;
            sprite = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sprite.SetData(new[] { Color.White });
            this.length = length;
            this.angleRad = angleRad;
            to = MathUtil.EndPointOfLine(from, length, this.angleRad);
            origin = new Vector2(0f, 0.5f);
            scale = new Vector2(length, thickness);
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, from, null, color, angleRad, origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

    }
}
