using GameEngine2D.Engine.Source.Entities.Abstract;
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
        public BoxTrigger(int width, int height, Vector2 positionOffset = default(Vector2), string tag = "", bool showTrigger = false) : this(width, height, positionOffset, tag)
        {
            DEBUG_DISPLAY_TRIGGER = showTrigger;
        }
#endif

        public BoxTrigger(int width, int height, Vector2 positionOffset = default(Vector2), string tag = "") : base(positionOffset, tag)
        {
            x1 = 0;
            y1 = 0;
            x2 = width;
            y2 = height;
        }

        public override bool IsInsideTrigger(IGameObject otherObject)
        {
            return otherObject.Transform.X >= Position.X + x1 && otherObject.Transform.X <= Position.X + x2 && otherObject.Transform.Y >= Position.Y + y1 && otherObject.Transform.Y <= Position.Y + y2;
        }

#if DEBUG
        protected override void CreateDebugVisual()
        {
            if (DEBUG_DISPLAY_TRIGGER)
            {
                Line lineX1 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), Color.Red);
                Line lineY1 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), Color.Red);
                Line lineX2 = new Line(Owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
                Line lineY2 = new Line(Owner, new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
            }
        }
#endif
    }
}
