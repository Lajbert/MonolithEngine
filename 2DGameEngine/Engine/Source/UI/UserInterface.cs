using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.UI
{
    public class UserInterface
    {
        private List<IUIElement> elements = new List<IUIElement>();

        private List<IUIElement> newElements = new List<IUIElement>();
        private List<IUIElement> removedElements = new List<IUIElement>();

        public void AddUIElement(IUIElement newElement)
        {
            newElements.Add(newElement);
        }

        public void RemoveUIElement(IUIElement toRemove)
        {
            removedElements.Add(toRemove);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (IUIElement element in elements)
            {
                element.Draw(spriteBatch, gameTime);
            }

            if (newElements.Count > 0)
            {
                foreach (IUIElement element in newElements)
                {
                    elements.Add(element);
                }

                newElements.Clear();
            }

            if (removedElements.Count > 0)
            {
                foreach (IUIElement element in removedElements)
                {
                    elements.Remove(element);
                }

                removedElements.Clear();
            }
        }

        public void Update()
        {
            foreach (IUIElement element in elements)
            {
                element.Update();
            }

            if (newElements.Count > 0)
            {
                foreach (IUIElement element in newElements)
                {
                    elements.Add(element);
                }

                newElements.Clear();
            }

            if (removedElements.Count > 0)
            {
                foreach (IUIElement element in removedElements)
                {
                    elements.Remove(element);
                }

                removedElements.Clear();
            }
        }
    }
}
