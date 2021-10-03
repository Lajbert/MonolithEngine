using Microsoft.Xna.Framework;
using MonolithEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample
{

    class MobileButtonPanel
    {
        private List<SelectableImage> buttons = new List<SelectableImage>();

        public MobileButtonPanel(Hero hero)
        {
            SelectableImage moveLeftButton = new SelectableImage(Assets.GetTexture("LeftArrow"), null, new Vector2(100, 600), default, 9);
            moveLeftButton.OnClick = hero.MoveLeft;
            buttons.Add(moveLeftButton);

            SelectableImage moveRightButton = new SelectableImage(Assets.GetTexture("RightArrow"), null, new Vector2(250, 600), default, 9);
            moveRightButton.OnClick = hero.MoveRight;
            buttons.Add(moveRightButton);

            SelectableImage jumpButton = new SelectableImage(Assets.GetTexture("XButton"), null, new Vector2(1050, 600), default, 9);
            jumpButton.OnClick = hero.Jump;
            buttons.Add(jumpButton);

            /*SelectableImage menuButton = new SelectableImage(Assets.GetTexture("MenuButton"), null, new Vector2(150, 50), default, 4);
            menuButton.OnClick = hero.Jump;
            buttons.Add(menuButton);*/
        }

        public List<SelectableImage> GetButtons()
        {
            return new List<SelectableImage>(buttons);
        }
    
    }
}
