using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Trigger
{
    public abstract class AbstractTrigger : ITrigger
    {
        protected Entity Owner;
        private string tag;
        protected Vector2 PositionOffset;

#if DEBUG
        private bool showDebug = false;
        public bool DEBUG_DISPLAY_TRIGGER
        {
            get => showDebug;
            set
            {
                if (value != showDebug)
                {
                    CreateDebugVisual();
                }
                showDebug = value;
            }
        }
#endif

        public Vector2 Position
        {
            get => Owner.Position + PositionOffset;
        }

        public AbstractTrigger(Entity owner, Vector2 positionOffset = default(Vector2), string tag = null)
        {
            Owner = owner;
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

#if DEBUG
        protected abstract void CreateDebugVisual();
#endif
    }
}
