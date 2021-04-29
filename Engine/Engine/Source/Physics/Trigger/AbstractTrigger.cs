using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    public abstract class AbstractTrigger : ITrigger
    {
        protected Entity owner;
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
                    showDebug = value;
                    CreateDebugVisual();
                }
                showDebug = value;
            }
        }
#endif

        public Vector2 Position
        {
            get => owner.Transform.Position + PositionOffset;
        }
        public bool UniquePerEntity { get; set; }

        public AbstractTrigger(Entity owner, Vector2 positionOffset = default, string tag = "")
        {
            this.owner = owner;
            PositionOffset = positionOffset;
            this.tag = tag;
            UniquePerEntity = false;
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
