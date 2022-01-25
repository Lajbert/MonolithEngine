using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample
{
    class ControlsPopup
    {

        private float Y_PADDING = 2f;
        private float X_PADDING = 1f;

        private Action removeButtonsAction;
        private Action addButtonsAction;

        public ControlsPopup(AbstractScene scene, Vector2 position)
        {
            if (scene is Level1Scene)
            {
                removeButtonsAction = (scene as Level1Scene).RemoveButtons;
                addButtonsAction = (scene as Level1Scene).AddButtons;
            }
            else if (scene is Level2Scene)
            {
                removeButtonsAction = (scene as Level2Scene).RemoveButtons;
                addButtonsAction = (scene as Level2Scene).AddButtons;
            }

            removeButtonsAction.Invoke();

            Image transparentBG = new Image(Assets.GetTexture2D("TransparentBG"), null, Vector2.Zero);
            scene.UI.AddUIElement(transparentBG);

            float frameScale = 6f;
            Texture2D frameTexture = Assets.GetTexture2D("UIFrame_Big");
            Image frame = new Image(frameTexture, null, position);
            frame.Scale = frameScale;
            frame.OwnPosition -= new Vector2(frameTexture.Width / 2, frameTexture.Height / 2) * frameScale;
            scene.UI.AddUIElement(frame);
            scene.LayerManager.Paused = true;
            
            float contentScale = 4f;

            Vector2 pos = new Vector2(150, 50);
            PNGFontRenderer fontRenderer = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "move\nclimb\njump\nattack\nslide\nlift box", pos, frame);
            fontRenderer.LetterSpacingOffset += new Vector2(-1, 7f);
            fontRenderer.Scale = 4f;

            Texture2D leftArrow = Assets.GetTexture2D("LeftArrow");
            Image leftButton = new Image(Assets.GetTexture2D("LeftArrow"), frame, new Vector2(40, 40), scale: contentScale);

            Texture2D rightArrow = Assets.GetTexture2D("RightArrow");
            Image rightButton = new Image(rightArrow, leftButton, new Vector2(leftArrow.Width + X_PADDING, 0) * contentScale, scale: contentScale);

            Texture2D upArrow = Assets.GetTexture2D("UpArrow");
            Image upButton = new Image(upArrow, leftButton, new Vector2(0, leftArrow.Height + Y_PADDING) * contentScale, scale: contentScale);

            Texture2D downArrow = Assets.GetTexture2D("DownArrow");
            Image downButton= new Image(downArrow, upButton, new Vector2(upArrow.Width + X_PADDING, 0) * contentScale, scale: contentScale);

            Texture2D xTexture = Assets.GetTexture2D("XButton");
            Image xButton = new Image(xTexture, downButton, new Vector2(0, downArrow.Height + Y_PADDING) * contentScale, scale: contentScale);

            Texture2D squareTexture = Assets.GetTexture2D("SquareButton");
            Image squareButton = new Image(squareTexture, xButton, new Vector2(0, xTexture.Height + Y_PADDING + 1) * contentScale, scale: contentScale);

            Texture2D circleTexture = Assets.GetTexture2D("CircleButton");
            Image circleButton = new Image(circleTexture, squareButton, new Vector2(0, squareTexture.Height + Y_PADDING + 1) * contentScale, scale: contentScale);

            Texture2D triangleTexture = Assets.GetTexture2D("TriangleButton");
            Image triangleButton = new Image(triangleTexture, circleButton, new Vector2(0, squareTexture.Height + Y_PADDING + 1) * contentScale, scale: contentScale);

            Texture2D texture = Assets.GetTexture2D("ContinueButton");
            Vector2 buttonPos = new Vector2(((frame.ImageTexture.Width / 2) * frame.Scale), 410);
            GameButton continueButton = new GameButton(texture, buttonPos, scale: 3, frame, true);

            continueButton.OnClick = () => scene.LayerManager.Paused = false;
            continueButton.OnClick += () => scene.UI.RemoveUIElement(frame);
            continueButton.OnClick += () => scene.UI.RemoveUIElement(transparentBG);
            continueButton.OnClick += addButtonsAction;
            continueButton.SetUserInterface(scene.UI);
        }
    }
}
