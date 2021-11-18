using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Class for updating and drawing UI elements for a given scene.
    /// </summary>
    public class UserInterface
    {
        private List<IUIElement> elements = new List<IUIElement>();

        private List<IUIElement> newElements = new List<IUIElement>();
        private List<IUIElement> removedElements = new List<IUIElement>();

        private IUIElement selectedElement;
        private MouseState currentMouseState;
        private MouseState prevMouseState;

        public void OnResolutionChanged()
        {
            foreach (IUIElement element in elements)
            {
                if (element is SelectableUIElement)
                {
                    (element as SelectableUIElement).OnResolutionChanged();
                }
            }
        }

        public void AddUIElement(IUIElement newElement)
        {
            newElements.Add(newElement);
            if (newElement is SelectableUIElement)
            {
                (newElement as SelectableUIElement).SetUserInterface(this);
            }
        }

        public void RemoveUIElement(IUIElement toRemove)
        {
            removedElements.Add(toRemove);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            HandleNewElements();

            foreach (IUIElement element in elements)
            {
                element.Draw(spriteBatch);
            }
        }

        public void SelectElement(IUIElement selectedElement)
        {
            this.selectedElement = selectedElement;
        }

        public void DeselectElement(IUIElement deselected)
        {
            if (selectedElement != null && selectedElement.Equals(deselected)) {
                selectedElement = null;
            }
        }

        public void Update()
        {
            if (MonolithGame.Platform.IsMobile())
            {
                foreach (IUIElement element in elements)
                {
                    TouchCollection state = TouchPanel.GetState();
                    element.Update(state);
                }

                HandleNewElements();
            }
            else
            {
                currentMouseState = Mouse.GetState();

                if (selectedElement != null && currentMouseState.LeftButton == ButtonState.Pressed && (prevMouseState == null || prevMouseState.LeftButton != ButtonState.Pressed))
                {
                    if (selectedElement is SelectableUIElement)
                    {
                        (selectedElement as SelectableUIElement).OnClick();
                    }
                }

                foreach (IUIElement element in elements)
                {
                    element.Update(currentMouseState.Position);
                }

                HandleNewElements();

                prevMouseState = currentMouseState;
            }

        }

        internal void HandleNewElements()
        {
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

        public void Clear()
        {
            elements.Clear();
            newElements.Clear();
            removedElements.Clear();
        }
    }
}
