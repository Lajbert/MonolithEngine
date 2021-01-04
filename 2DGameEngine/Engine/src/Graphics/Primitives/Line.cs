using GameEngine2D.Engine.src.Layer;
using GameEngine2D.Entities;
using GameEngine2D.src;
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
    public class Line : Entity
    {
        private Vector2 Origin;
        private Vector2 Scale;

        public Vector2 From;
        public Vector2 To;

        private Vector2 fromSaved;
        private Vector2 toSaved;

        private Color color;
        private float thickness;

        private float angleRad;
        private float length;

        public Line(Entity parent, Vector2 from, Vector2 to, Color color, float thickness = 1f) : base(Scene.Instance.EntityLayer, parent, from, null)
        {
            this.From = fromSaved = from;
            this.To = toSaved = to;
            this.thickness = thickness;
            this.color = color;
            Sprite = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Sprite.SetData(new[] { Color.White });
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.AngleFromVectors(from, to);
            Origin = new Vector2(0f, 0f);
            Scale = new Vector2(length, thickness);
        }

        public Line(Entity parent, Vector2 from, float angleRad, float length, Color color, float thickness = 1f) : base(Scene.Instance.EntityLayer, parent, from, null)
        {
            this.From = fromSaved = from;
            this.thickness = thickness;
            this.color = color;
            Sprite = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Sprite.SetData(new[] { Color.White });
            this.length = length;
            this.angleRad = angleRad;
            To = toSaved = MathUtil.EndPointOfLine(from, length, this.angleRad);
            Origin = new Vector2(0f, 0f);
            Scale = new Vector2(length, thickness);
        }

        public void SetEnd(Vector2 end)
        {
            To = end;
            length = Vector2.Distance(From, To);
            angleRad = MathUtil.AngleFromVectors(From, To);
            Scale = new Vector2(length, thickness);
        }

        public void Reset()
        {
            From = fromSaved;
            To = toSaved;
            length = Vector2.Distance(From, To);
            angleRad = MathUtil.AngleFromVectors(From, To);
            Scale = new Vector2(length, thickness);
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            SpriteBatch.Begin();
            SpriteBatch.Draw(Sprite, position, null, color, angleRad, Origin, Scale, SpriteEffects.None, 0);
            SpriteBatch.End();
        }

        protected override void SetRayBlockers()
        {
            RayBlockerLines.Add((From, To));
        }

    }
}
