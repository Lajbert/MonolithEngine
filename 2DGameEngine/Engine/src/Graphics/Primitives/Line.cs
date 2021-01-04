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
    class Line : Entity
    {
        private Vector2 origin;
        private Vector2 scale;

        public Vector2 from;
        public Vector2 to;

        public Vector2 fromSaved;
        public Vector2 toSaved;

        private Color color;
        private float thickness;

        private float angleRad;
        private float length;

        public Line(Entity parent, Vector2 from, Vector2 to, Color color, float thickness = 1f) : base(Scene.Instance.GetEntityLayer(), parent, from, null)
        {
            this.from = fromSaved = from;
            this.to = toSaved = to;
            this.thickness = thickness;
            this.color = color;
            Sprite = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Sprite.SetData(new[] { Color.White });
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.AngleFromVectors(from, to);
            origin = new Vector2(0f, 0f);
            scale = new Vector2(length, thickness);
        }

        public Line(Entity parent, Vector2 from, float angleRad, float length, Color color, float thickness = 1f) : base(Scene.Instance.GetEntityLayer(), parent, from, null)
        {
            this.from = fromSaved = from;
            this.thickness = thickness;
            this.color = color;
            Sprite = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Sprite.SetData(new[] { Color.White });
            this.length = length;
            this.angleRad = angleRad;
            to = toSaved = MathUtil.EndPointOfLine(from, length, this.angleRad);
            origin = new Vector2(0f, 0f);
            scale = new Vector2(length, thickness);
        }

        public void SetEnd(Vector2 end)
        {
            to = end;
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.AngleFromVectors(from, to);
            scale = new Vector2(length, thickness);
        }

        public void Reset()
        {
            from = fromSaved;
            to = toSaved;
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.AngleFromVectors(from, to);
            scale = new Vector2(length, thickness);
        }

        protected override void DrawSprite(Vector2 position)
        {
            //base.DrawSprite(position);
            SpriteBatch.Begin();
            SpriteBatch.Draw(Sprite, position, null, color, angleRad, origin, scale, SpriteEffects.None, 0);
            SpriteBatch.End();
        }

        protected override void SetRayBlockers()
        {
            RayBlockerLines.Add((from, to));
        }

    }
}
