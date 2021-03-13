using GameEngine2D.Engine.Source.Components;
using GameEngine2D.Engine.Source.Entities.Abstract;
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
        private string tag = "";
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
            get => Owner.Transform.Position + PositionOffset;
        }
        public bool UniquePerEntity { get; set; }

        public AbstractTrigger(Vector2 positionOffset = default, string tag = "")
        {
            PositionOffset = positionOffset;
            this.tag = tag;
            UniquePerEntity = false;
        }

        public void SetOwner(Entity owner)
        {
            Owner = owner;
#if DEBUG
            CreateDebugVisual();
#endif
        }

        public abstract bool IsInsideTrigger(IGameObject otherObject);

        public string GetTag()
        {
            return tag;
        }

        public void SetTag(string tag)
        {
            this.tag = tag;
        }

        Type IComponent.GetComponentType()
        {
            return typeof(ITrigger);
        }

#if DEBUG
        protected abstract void CreateDebugVisual();
#endif
    }
}
