using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Asset
{
    public class MonolithTexture
    {
        private Texture2D texture;

        private Rectangle autoBoundingBox;

        private Vector2 center;

        public Vector2 bottomMiddle;

        public MonolithTexture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
