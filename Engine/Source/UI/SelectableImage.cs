﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        public SelectableImage(Texture2D texture, Texture2D selectedImage = null, Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, float rotation = 0f, int depth = 1, Color color = default) : base (texture, position, sourceRectangle, scale, rotation, depth, color)
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
            if (Config.ANDROID)
            {
                if (IsMouseOver(mousePosition)) {
                    OnClick();
                }
            }
            else
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
