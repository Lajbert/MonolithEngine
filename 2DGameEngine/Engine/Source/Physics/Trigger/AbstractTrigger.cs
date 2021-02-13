using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Trigger
{
    public abstract class AbstractTrigger : ITrigger
    {
        private Entity owner;
        private string tag;
        protected Vector2 PositionOffset;

        public Vector2 Position
        {
            get => owner.Position + PositionOffset;
        }

        public AbstractTrigger(Entity owner, Vector2 positionOffset = default(Vector2), string tag = null)
        {
            this.owner = owner;
            PositionOffset = positionOffset;
            this.tag = tag;
        }

        public abstract bool IsInsideTrigger(Vector2 point);

        public string GetTag()
        {
            return tag;
        }

        public void SetTag(string tag)
        {
            this.tag = tag;
        }
    }
}
