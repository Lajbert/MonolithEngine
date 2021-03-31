using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.UI.Interface;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonolithEngine.Engine.Source.UI
{
    public class SelectableImage : Image, SelectableUIElement
    {
        public bool IsSelectedByCursor = false;
        public bool IsSelectedByButtons = false;

        private Texture2D selectedImageTexture;
        private Rectangle selectionBox;

        public UserInterface userInterface;

        public Action OnClick;

        public SelectableImage(Texture2D texture, Texture2D selectedImage = null, Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, float rotation = 0f, int depth = 1, Color color = default) : base (texture, position, sourceRectangle, scale, rotation, depth, color)
        {
            selectedImageTexture = selectedImage;
            if (sourceRectangle == default)
            {
                selectionBox = new Rectangle((int)(position.X + sourceRectangle.X), (int)(position.Y + sourceRectangle.Y), ImageTexture.Width * (int)scale, ImageTexture.Height * (int)scale);
            } else
            {
                selectionBox = new Rectangle((int)(position.X + sourceRectangle.X), (int)(position.Y + sourceRectangle.Y), sourceRectangle.Width * (int)scale, sourceRectangle.Height * (int)scale);
            }

            OnClick = () =>
            {
                Logger.Info("------------- CLICK -----------");
            };
        }

        private bool IsMouseOver(Point mousePosition)
        {
            return selectionBox.Contains(mousePosition);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsSelectedByCursor || IsSelectedByButtons)
            {
                spriteBatch.Draw(selectedImageTexture, Position, SourceRectangle, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffect, Depth);
            } else
            {
                base.Draw(spriteBatch);
            }
        }

        public override void Update(Point mousePosition = default)
        {
            if (mousePosition != default && IsMouseOver(mousePosition))
            {
                IsSelectedByCursor = true;
                userInterface.SelectElement(this);
            } 
            else
            {
                IsSelectedByCursor = false;
                userInterface.DeselectElement(this);
            }
        }

        void SelectableUIElement.OnClick()
        {
            OnClick.Invoke();
        }

        public void SetUserInterface(UserInterface userInterface)
        {
            this.userInterface = userInterface;
        }
    }
}
