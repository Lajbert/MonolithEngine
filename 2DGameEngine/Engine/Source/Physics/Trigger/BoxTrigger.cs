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

        public BoxTrigger(Entity owner, int width, int height, Vector2 positionOffset = default(Vector2), string tag = null) : base(owner, positionOffset, tag)
        {
            x1 = 0;
            y1 = 0;
            x2 = width;
            y2 = height;
        }

        public override bool IsInsideTrigger(Vector2 point)
        {
            return point.X >= Position.X + x1 && point.X <= Position.X + x2 && point.Y >= Position.Y + y1 && point.Y <= Position.Y + y2;
        }

#if DEBUG
        protected override void CreateDebugVisual()
        {
            lineX1 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), Color.Red);
            lineY1 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), Color.Red);
            lineX2 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
            lineY2 = new Line(Owner, new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
        }
#endif
    }
}
