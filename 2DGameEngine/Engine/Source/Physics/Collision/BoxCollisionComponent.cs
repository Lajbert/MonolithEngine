using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public class BoxCollisionComponent : AbstractCollisionComponent
    {

        public float Width = 0;
        public float Height = 0;

        public BoxCollisionComponent(IColliderEntity owner, float width, float height, Vector2 positionOffset = default(Vector2)) : base(ColliderType.BOX, owner, positionOffset)
        {
            Width = width;
            Height = height;
        }

        public override bool Overlaps(IColliderEntity otherCollider)
        {
            if (otherCollider.GetCollisionComponent().GetType() == ColliderType.BOX)
            {
                BoxCollisionComponent otherBox = otherCollider as BoxCollisionComponent;

                return Position.X < otherBox.Position.X + otherBox.Width &&
                   Position.X + Width > otherBox.Position.X &&
                   Position.Y < otherBox.Position.Y + otherBox.Height &&
                   Position.Y + Height > otherBox.Position.Y;
            }
            else if (otherCollider.GetCollisionComponent().GetType() == ColliderType.CIRCLE)
            {
                return otherCollider.GetCollisionComponent().Overlaps(owner);
            }
            throw new Exception("Unknown collider type");
        }
    }
}
