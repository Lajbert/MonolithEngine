using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Base class representing UI elements.
    /// UI elements move with the camera and are unaffected by the zoom level.
    /// </summary>
    public abstract class AbstractUIElement : IUIElement
    {
        protected IUIElement Parent;
        protected HashSet<IUIElement> Children = new HashSet<IUIElement>();
        public Vector2 OwnPosition;
        public Vector2 Origin = Vector2.Zero;

        public AbstractUIElement(Vector2 position, IUIElement parent = null)
        {
            if (parent == null)
            {
                float x = VideoConfiguration.RESOLUTION_WIDTH * (position.X / 100) / Config.SCALE;
                float y = VideoConfiguration.RESOLUTION_HEIGHT * (position.Y / 100) / Config.SCALE;
                OwnPosition = new Vector2(x, y);
            }
            else
            {
                Parent = parent;
                Parent.AddChild(this);
                OwnPosition = position;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Children.Count > 0)
            {
                foreach (IUIElement child in Children)
                {
                    child.Draw(spriteBatch);
                }
            }
        }

        public HashSet<IUIElement> GetChildren()
        {
            return Children;
        }

        public IUIElement GetParent()
        {
            return Parent;
        }

        public Vector2 GetPosition()
        {
            if (Parent != null)
            {
                return (Parent.GetPosition() + OwnPosition);// * Config.SCALE;
            }
            return OwnPosition;// * Config.SCALE;
        }

        public virtual void Update(Point mousePosition = default)
        {
            if (Children.Count > 0)
            {
                foreach (IUIElement child in Children)
                {
                    child.Update(mousePosition);
                }
            }
        }

        public virtual void Update(TouchCollection touchLocations)
        {
            if (Children.Count > 0)
            {
                foreach (IUIElement child in Children)
                {
                    child.Update(touchLocations);
                }
            }
        }

        public virtual void FixedUpdate()
        {
            if (Children.Count > 0)
            {
                foreach (IUIElement child in Children)
                {
                    child.FixedUpdate();
                }
            }
        }

        public void AddChild(IUIElement child)
        {
            Children.Add(child);
        }

        public void SetParent(IUIElement parent)
        {
            Parent = parent;
        }
    }
}
