using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample
{
    class SlideTutorialPopup
    {

        private Action removeButtonsAction;
        private Action addButtonsAction;

        public SlideTutorialPopup(AbstractScene scene, Vector2 position)
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
            Texture2D frameTexture = Assets.GetTexture2D("UIFrame_Long");
            Image frame = new Image(frameTexture, null, position);
            frame.Scale = frameScale;
            frame.OwnPosition -= new Vector2(frameTexture.Width / 2, frameTexture.Height / 2) * frameScale;
            scene.UI.AddUIElement(frame);
            scene.LayerManager.Paused = true;

            float contentScale = 4f;

            Vector2 pos = new Vector2(40, 35);
            PNGFontRenderer fontRenderer = new PNGFontRenderer(Assets.GetPNGFontSheet("PixelFont"), "press   to slide", pos, frame);
            fontRenderer.LetterSpacingOffset += new Vector2(-1, 7f);
            fontRenderer.Scale = 4f;

            Texture2D circleTexture = Assets.GetTexture2D("CircleButton");
            Image slideButton = new Image(circleTexture, frame, new Vector2(37, 6) * contentScale, scale: contentScale);

            Texture2D texture = Assets.GetTexture2D("ContinueButton");
            Vector2 buttonPos = new Vector2(((frame.ImageTexture.Width / 2) * frame.Scale), (frame.ImageTexture.Height * frame.Scale + texture.Height * contentScale));
            GameButton continueButton = new GameButton(texture, buttonPos, scale: 3, frame, true);

            continueButton.OnClick = () => scene.LayerManager.Paused = false;
            continueButton.OnClick += () => scene.UI.RemoveUIElement(frame);
            continueButton.OnClick += () => scene.UI.RemoveUIElement(transparentBG);
            continueButton.OnClick += addButtonsAction;
            continueButton.SetUserInterface(scene.UI);
        }
    }
}
