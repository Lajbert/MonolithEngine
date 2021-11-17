﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// Represents a line. 
    /// WARNING: bad performance, only for debugging purposes!
    /// </summary>
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

        public Line(AbstractScene scene, Entity parent, Vector2 from, Vector2 to, Color color, float thickness = 1f) : base(scene.LayerManager.EntityLayer, parent, from)
        {
            this.From = fromSaved = from;
            this.To = toSaved = to;
            this.thickness = thickness;
            this.color = color;
            SetSprite(new MonolithTexture(AssetUtil.CreateRectangle(1, Color.White)));
            length = Vector2.Distance(from, to);
            angleRad = MathUtil.RadFromVectors(from, to);
            Origin = new Vector2(0f, 0f);
            Scale = new Vector2(length, thickness);
        }

        public Line(AbstractScene scene, Entity parent, Vector2 from, float angleRad, float length, Color color, float thickness = 1f) : base(scene.LayerManager.EntityLayer, parent, from)
        {
            this.From = fromSaved = from;
            this.thickness = thickness;
            this.color = color;
            SetSprite(new MonolithTexture(AssetUtil.CreateRectangle(1, Color.White)));
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
            angleRad = MathUtil.RadFromVectors(From, To);
            Scale = new Vector2(length, thickness);
        }

        public void Reset()
        {
            From = fromSaved;
            To = toSaved;
            length = Vector2.Distance(From, To);
            angleRad = MathUtil.RadFromVectors(From, To);
            Scale = new Vector2(length, thickness);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetComponent<Sprite>().Texture.GetTexture2D(), Transform.Position, null, color, angleRad, Origin, Scale, SpriteEffects.None, 0);
        }

        protected override void SetRayBlockers()
        {
            RayBlockerLines.Add((From, To));
        }

    }
}
