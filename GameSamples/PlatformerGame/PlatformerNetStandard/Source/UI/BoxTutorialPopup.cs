using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample
{
    class BoxTutorialPopup
    {

        private float Y_PADDING = 2f;
        private float X_PADDING = 1f;

        private Action removeButtonsAction;
        private Action addButtonsAction;

        public BoxTutorialPopup(AbstractScene scene, Vector2 position)
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
            Texture2D frameTexture = Assets.GetTexture2D("UIFrame_Medium");
            Image frame = new Image(frameTexture, null, position);
            frame.Scale = frameScale;
            frame.OwnPosition -= new Vector2(frameTexture.Width / 2, frameTexture.Height / 2) * frameScale;
            scene.UI.AddUIElement(frame);
            scene.LayerManager.Paused = true;
            
            float contentScale = 4f;

            Vector2 pos = new Vector2(40, 40);
            PNGFontRenderer fontRenderer = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "press   to lift box\npress   to throw box", pos, frame);
            fontRenderer.LetterSpacingOffset += new Vector2(-1, 7f);
            fontRenderer.Scale = 4f;

            Texture2D triangleTexture = Assets.GetTexture2D("TriangleButton");
            Image triangleButton = new Image(triangleTexture, frame, new Vector2(37, 8) * contentScale, scale: contentScale);

            Texture2D squareTexture = Assets.GetTexture2D("SquareButton");
            Image squareButton = new Image(squareTexture, triangleButton, new Vector2(0, squareTexture.Height + Y_PADDING + 1) * contentScale, scale: contentScale);

            Texture2D texture = Assets.GetTexture2D("ContinueButton");
            Vector2 buttonPos = new Vector2(((frame.ImageTexture.Width / 2) * frame.Scale), 300);
            GameButton continueButton = new GameButton(texture, buttonPos, scale: 3, frame, true);

            AnimatedImage animImg = new AnimatedImage(Assets.GetAnimationTexture("BoxTutorial"), new Vector2(60, 35)  * contentScale, 40, parent: frame);
            animImg.Scale = 0.8f;

            continueButton.OnClick = () => scene.LayerManager.Paused = false;
            continueButton.OnClick += () => scene.UI.RemoveUIElement(frame);
            continueButton.OnClick += () => scene.UI.RemoveUIElement(transparentBG);
            continueButton.OnClick += addButtonsAction;
            continueButton.SetUserInterface(scene.UI);
        }
    }
}
