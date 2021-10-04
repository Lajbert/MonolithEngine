using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// An image that can be selected and deselected.
    /// </summary>
    public class SelectableImage : Image, SelectableUIElement
    {
        public bool IsHoveredOver = false;
        public bool IsSelected = false;

        private Texture2D selectedImageTexture;
        private Rectangle unscaledSelectionBox;
        private Rectangle selectionBox;

        public UserInterface userInterface;

        public Action OnClick;

        public Action HoverStartedAction;

        public Action HoverStoppedAction;

        public string HoverSoundEffectName;

        public string SelectSoundEffectName;

        private bool fireOnHold;

        private bool isBeingFired = false;

        public SelectableImage(Texture2D texture, Texture2D selectedImage = null, Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, bool fireOnHold = false, float rotation = 0f, int depth = 1, Color color = default) : base (texture, position, sourceRectangle, scale, rotation, depth, color)
        {
            selectedImageTexture = selectedImage;
            if (sourceRectangle == default)
            {
                unscaledSelectionBox = new Rectangle((int)position.X + sourceRectangle.X, (int)position.Y + sourceRectangle.Y, (int)(ImageTexture.Width * scale), (int)(ImageTexture.Height * scale));
            } 
            else
            {
                unscaledSelectionBox = new Rectangle((int)position.X + sourceRectangle.X, (int)position.Y + sourceRectangle.Y, (int)(sourceRectangle.Width * scale), (int)(sourceRectangle.Height * scale));
            }

            this.fireOnHold = fireOnHold;

            OnResolutionChanged();
        }

        public void OnResolutionChanged()
        {
            selectionBox = new Rectangle((int)(unscaledSelectionBox.X * Config.SCALE), (int)(unscaledSelectionBox.Y * Config.SCALE), (int)(unscaledSelectionBox.Width * Config.SCALE), (int)(unscaledSelectionBox.Height * Config.SCALE));
        }

        private bool IsMouseOver(Point mousePosition)
        {
            return selectionBox.Contains(mousePosition);
        }

        private bool IsMouseOver(Vector2 mousePosition)
        {
            return selectionBox.Contains(mousePosition);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsHoveredOver || IsSelected)
            {
                spriteBatch.Draw(selectedImageTexture, GetPosition(), SourceRectangle, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffect, Depth);
            } else
            {
                base.Draw(spriteBatch);
            }
        }

        public override void Update(Point mousePosition = default)
        {
            if (mousePosition != default && IsMouseOver(mousePosition))
            {
                if (!IsHoveredOver)
                {
                    if (HoverSoundEffectName != null)
                    {
                        AudioEngine.Play(HoverSoundEffectName);
                    }
                    HoverStartedAction?.Invoke();
                }
                IsHoveredOver = true;
                userInterface.SelectElement(this);
            }
            else
            {
                if (IsHoveredOver)
                {
                    HoverStoppedAction?.Invoke();
                }
                IsHoveredOver = false;
                userInterface.DeselectElement(this);
            }
        }

        public override void Update(TouchCollection touchLocations)
        {
            bool wasTouched = false;
            foreach (TouchLocation touch in touchLocations)
            {
                if (IsMouseOver(touch.Position))
                {
                    wasTouched = true;
                    if (fireOnHold)
                    {
                        OnClick();
                    }
                    else if (!isBeingFired)
                    {
                        OnClick();
                    }
                    isBeingFired = true;

                    break;
                }
            }
            if (!wasTouched)
            {
                isBeingFired = false;
            }
        }

        void SelectableUIElement.OnClick()
        {
            if (SelectSoundEffectName != null)
            {
                AudioEngine.Play(SelectSoundEffectName);
            }
            OnClick.Invoke();
        }

        public void SetUserInterface(UserInterface userInterface)
        {
            this.userInterface = userInterface;
        }
    }
}
