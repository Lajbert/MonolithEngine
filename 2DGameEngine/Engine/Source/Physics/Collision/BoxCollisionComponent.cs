using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;


#if DEBUG
using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Entities;
#endif

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public class BoxCollisionComponent : AbstractCollisionComponent
    {

        public float Width = 0;
        public float Height = 0;

#if DEBUG
        private Line lineX1;
        private Line lineX2;
        private Line lineY1;
        private Line lineY2;
#endif

        public BoxCollisionComponent(IColliderEntity owner, float width, float height, Vector2 positionOffset = default(Vector2)) : base(ColliderType.BOX, owner, positionOffset)
        {
            Width = width;
            Height = height;
        }


        public override bool CollidesWith(IColliderEntity otherCollider)
        {
            if (otherCollider.GetCollisionComponent().GetType() == ColliderType.BOX)
            {
                BoxCollisionComponent otherBox = otherCollider.GetCollisionComponent() as BoxCollisionComponent;

                return Position.X <= otherBox.Position.X + otherBox.Width &&
                   Position.X + Width >= otherBox.Position.X &&
                   Position.Y <= otherBox.Position.Y + otherBox.Height &&
                   Position.Y + Height >= otherBox.Position.Y;
            }
            else if (otherCollider.GetCollisionComponent().GetType() == ColliderType.CIRCLE)
            {
                return otherCollider.GetCollisionComponent().CollidesWith(owner);
            }
            throw new Exception("Unknown collider type");
        }

#if DEBUG
        protected override void CreateDebugVisual()
        {
            float x1 = 0;
            float y1 = 0;
            float x2 = Width;
            float y2 = Height;
            lineX1 = new Line(owner as Entity, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), Color.Red);
            lineY1 = new Line(owner as Entity, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), Color.Red);
            lineX2 = new Line(owner as Entity, new Vector2(PositionOffset.X + x1, PositionOffset.Y + y2), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
            lineY2 = new Line(owner as Entity, new Vector2(PositionOffset.X + x2, PositionOffset.Y + y1), new Vector2(PositionOffset.X + x2, PositionOffset.Y + y2), Color.Red);
        }
#endif
    }
}
