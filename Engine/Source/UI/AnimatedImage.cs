using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonolithEngine
{
    public class AnimatedImage : SpriteSheetAnimation, IUIElement
    {

        private IUIElement parent;
        private HashSet<IUIElement> children = new HashSet<IUIElement>();

        public AnimatedImage(AnimationTexture texture, Vector2 position, int framerate, SpriteEffects spriteEffect = SpriteEffects.None, IUIElement parent = null) : base(null, texture, framerate, spriteEffect)
        {
            this.parent = parent;
            Offset = position;
            Init(0);

            if (parent == null)
            {
                float x = VideoConfiguration.RESOLUTION_WIDTH * (position.X / 100) / Config.SCALE;
                float y = VideoConfiguration.RESOLUTION_HEIGHT * (position.Y / 100) / Config.SCALE;
                Offset = new Vector2(x, y);
            }
            else
            {
                this.parent = parent;
                parent.AddChild(this);
                Offset = parent.GetPosition() + position;
            }
        }

        public void AddChild(IUIElement child)
        {
            children.Add(child);
        }

        public HashSet<IUIElement> GetChildren()
        {
            return children;
        }

        void IUIElement.Draw(SpriteBatch spriteBatch)
        {
            Play(spriteBatch);
        }

        IUIElement IUIElement.GetParent()
        {
            return parent;
        }

        Vector2 IUIElement.GetPosition()
        {
            return Offset;
        }

        void IUIElement.Update(Point mousePosition)
        {
        }

        void IUIElement.Update(TouchCollection touchLocations)
        {
        }

        public void FixedUpdate()
        {
            base.Update();
        }

        public void SetParent(IUIElement parent)
        {
            this.parent = parent;
        }
    }
}
