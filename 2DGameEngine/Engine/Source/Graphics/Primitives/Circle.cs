using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Source;
using MonolithEngine.Source.GridCollision;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Graphics.Primitives
{
    public class Circle : Entity
    {
        private Vector2 center;
        private Color color;
        private float radius;
        private Vector2 offset;

        public Circle(Entity parent, Vector2 center, int radius, Color color) : base(LayerManager.Instance.EntityLayer, parent, center, null)
        {
            SetSprite(TextureUtil.CreateCircle(radius, color));
            this.color = color;
            this.center = center;
            this.radius = radius;
            this.offset = new Vector2(radius, radius) / 2;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GetComponent<Sprite>().DrawOffset -= offset;
            base.Draw(spriteBatch, gameTime);
        }
    }
}
