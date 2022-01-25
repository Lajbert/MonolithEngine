using Microsoft.Xna.Framework;

namespace MonolithEngine
{

    /// <summary>
    /// Box trigger implementation: checks whether a point
    /// is inside the rectangle it defines.
    /// </summary>
    public class BoxTrigger : AbstractTrigger
    {
        private int x1;
        private int y1;
        private int x2;
        private int y2;

#if DEBUG
        public BoxTrigger(Entity owner, int width, int height, Vector2 positionOffset = default, string tag = "", bool showTrigger = false) : this(owner, width, height, positionOffset, tag)
        {
            DEBUG_DISPLAY_TRIGGER = showTrigger;
        }
#endif

        public BoxTrigger(Entity owner, int width, int height, Vector2 positionOffset = default, string tag = "") : base(owner, positionOffset, tag)
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
                Line lineX1 = new Line(owner.Scene, owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), Color.Red);
                Line lineY1 = new Line(owner.Scene, owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), Color.Red);
                Line lineX2 = new Line(owner.Scene, owner, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
                Line lineY2 = new Line(owner.Scene, owner, new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
            }
        }
#endif
    }
}
