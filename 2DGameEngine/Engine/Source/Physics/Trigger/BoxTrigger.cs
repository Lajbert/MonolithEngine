using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Trigger
{
    public class BoxTrigger : AbstractTrigger
    {
        private int x1;
        private int y1;
        private int x2;
        private int y2;

#if DEBUG
        private Line lineX1;
        private Line lineX2;
        private Line lineY1;
        private Line lineY2;
#endif

        public BoxTrigger(Entity owner, Rectangle bound, Vector2 positionOffset = default(Vector2), string tag = null) : base(owner, positionOffset, tag)
        {
            x1 = bound.X;
            y1 = bound.Y;
            x2 = bound.X + bound.Width;
            y2 = bound.Y + bound.Height;

        }

#if DEBUG
        public BoxTrigger(Entity owner, Rectangle bound, Vector2 positionOffset = default(Vector2), string tag = null, bool displayTrigger = false) : this(owner, bound, positionOffset, tag)
        {
            if (displayTrigger)
            {
                lineX1 = new Line(owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), Color.Red);
                lineY1 = new Line(owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), Color.Red);
                lineX2 = new Line(owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
                lineY2 = new Line(owner, new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
            }
        }
#endif

        public override bool IsInsideTrigger(Vector2 point)
        {
            return point.X >= Position.X + x1 && point.X <= Position.X + x2 && point.Y >= Position.Y + y1 && point.Y <= Position.Y + y2;
        }
    }
}
