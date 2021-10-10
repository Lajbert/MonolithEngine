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
            SelectableImage moveLeftButton = new SelectableImage(Assets.GetTexture("LeftArrow"), null, new Vector2(25, 600), default, 9, fireOnHold: true);
            moveLeftButton.OnClick = AddButtonAction(hero, hero.MoveLeft);
            buttons.Add(moveLeftButton);

            SelectableImage downButton = new SelectableImage(Assets.GetTexture("DownArrow"), null, new Vector2(150, 600), default, 9, fireOnHold: true);
            downButton.OnClick = AddButtonAction(hero, hero.ClimbDownOnLadder);
            buttons.Add(downButton);

            SelectableImage upButton = new SelectableImage(Assets.GetTexture("UpArrow"), null, new Vector2(150, 475), default, 9, fireOnHold: true);
            upButton.OnClick = AddButtonAction(hero, hero.ClimbUpOnLadder);
            buttons.Add(upButton);

            SelectableImage moveRightButton = new SelectableImage(Assets.GetTexture("RightArrow"), null, new Vector2(270, 600), default, 9, fireOnHold: true);
            moveRightButton.OnClick = AddButtonAction(hero, hero.MoveRight);
            buttons.Add(moveRightButton);

            SelectableImage punchButton = new SelectableImage(Assets.GetTexture("SquareButton"), null, new Vector2(950, 600), default, 9);
            punchButton.OnClick = AddButtonAction(hero, hero.AttackOrThrow);
            buttons.Add(punchButton);

            SelectableImage jumpButton = new SelectableImage(Assets.GetTexture("XButton"), null, new Vector2(1100, 600), default, 9);
            jumpButton.OnClick = AddButtonAction(hero, hero.Jump);
            buttons.Add(jumpButton);

            SelectableImage interactButton = new SelectableImage(Assets.GetTexture("TriangleButton"), null, new Vector2(1025, 500), default, 9);
            interactButton.OnClick = AddButtonAction(hero, hero.InteractWithItem);
            buttons.Add(interactButton);

            /*
             *   Assets.LoadTexture("LeftArrow", "MobileButtons/left_arrow");
                Assets.LoadTexture("RightArrow", "MobileButtons/right_arrow");
                Assets.LoadTexture("UpArrow", "MobileButtons/up_arrow");
                Assets.LoadTexture("DownArrow", "MobileButtons/down_arrow");
                Assets.LoadTexture("XButton", "MobileButtons/x_button");
                Assets.LoadTexture("SquareButton", "MobileButtons/square_button");
                Assets.LoadTexture("CircleButton", "MobileButtons/circle_button");
                Assets.LoadTexture("TriangleButton", "MobileButtons/triangle_button");
                Assets.LoadTexture("MenuButton", "MobileButtons/menu_button");
             * */
            /*SelectableImage menuButton = new SelectableImage(Assets.GetTexture("MenuButton"), null, new Vector2(150, 50), default, 4);
            menuButton.OnClick = hero.Jump;
            buttons.Add(menuButton);*/
        }

        private Action AddButtonAction(Hero hero, Action action)
        {
            Action result = () =>
            {
                if (!hero.GetComponent<UserInputController>().ControlsDisabled)
                {
                    action.Invoke();
                }
            };
            return result;
        }

        public List<SelectableImage> GetButtons()
        {
            return new List<SelectableImage>(buttons);
        }
    
    }
}
