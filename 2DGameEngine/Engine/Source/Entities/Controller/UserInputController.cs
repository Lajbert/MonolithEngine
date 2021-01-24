using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Controller
{
    public class UserInputController
    {

        private Dictionary<Keys, bool> pressedKeys = new Dictionary<Keys, bool>();
        private Dictionary<Buttons, bool> pressedButtons = new Dictionary<Buttons, bool>();
        private Dictionary<KeyMapping, Action<Vector2>> keyPressActions = new Dictionary<KeyMapping, Action<Vector2>>();
        private Dictionary<Keys, Action> keyReleaseActions = new Dictionary<Keys, Action>();
        private Dictionary<Buttons, Action> buttonReleaseActions = new Dictionary<Buttons, Action>();
        private KeyboardState currentKeyboardState;
        private KeyboardState prevKeyboardState;
        private GamePadState prevGamepadState;
        private MouseState mouseState;
        private GamePadState currentGamepadState;

        private int prevMouseScrollWheelValue = 0;
        private Action mouseWheelUpAction;
        private Action mouseWheelDownAction;

        private Vector2 leftThumbstick = Vector2.Zero;
        private Vector2 rightThumbStick = Vector2.Zero;

        public void RegisterKeyPressAction(Keys key, Buttons controllerButton, Action<Vector2> action, bool singlePressOnly = false)
        {
            keyPressActions.Add(new KeyMapping(key, controllerButton, singlePressOnly), action);
            pressedKeys[key] = false;
            pressedButtons[controllerButton] = false;
        }

        public void RegisterKeyReleaseAction(Keys key, Buttons controllerButton, Action action)
        {
            keyReleaseActions.Add(key, action);
            buttonReleaseActions.Add(controllerButton, action);
        }

        public void RegisterKeyPressAction(Buttons controllerButton, Action<Vector2> action, bool singlePressOnly = false)
        {
            keyPressActions.Add(new KeyMapping(null, controllerButton, singlePressOnly), action);
            pressedButtons[controllerButton] = false;
        }

        public void RegisterKeyPressAction(Keys key, Action<Vector2> action, bool singlePressOnly = false) {
            keyPressActions.Add(new KeyMapping(key, null, singlePressOnly), action);
            pressedKeys[key] = false;
        }

        public void RegisterMouseActions(Action wheelUpAction, Action wheelDownAction)
        {
            mouseWheelUpAction = wheelUpAction;
            mouseWheelDownAction = wheelDownAction;
        }

        public bool IsKeyPressed(Keys key)
        {
            return pressedKeys[key];
        }

        public void Update()
        {
            currentKeyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            currentGamepadState = GamePad.GetState(PlayerIndex.One);

            foreach (KeyValuePair<KeyMapping, Action<Vector2>> mapping in keyPressActions)
            {
                Keys? key = mapping.Key.Key;
                if (key.HasValue)
                {
                    if (currentKeyboardState.IsKeyDown(key.Value))
                    {
                        if (mapping.Key.SinglePressOnly && (prevKeyboardState != null && (prevKeyboardState == currentKeyboardState || pressedKeys[key.Value])))
                        {
                            continue;
                        }
                        pressedKeys[key.Value] = true;
                        mapping.Value.Invoke(Vector2.Zero);
                    }
                    else
                    {
                        if (pressedKeys[key.Value] && keyReleaseActions.ContainsKey(key.Value))
                        {
                            keyReleaseActions[key.Value].Invoke();
                        }
                        pressedKeys[key.Value] = false;
                    }
                }

                Buttons? button = mapping.Key.Button;
                if (button.HasValue)
                {
                    if (currentGamepadState.IsButtonDown(button.Value))
                    {
                        if (mapping.Key.SinglePressOnly && (prevGamepadState != null && (prevGamepadState == currentGamepadState || pressedButtons[button.Value])))
                        {
                            continue;
                        }
                        pressedButtons[button.Value] = true;
                        if (button.Value == Buttons.LeftThumbstickLeft || button.Value == Buttons.LeftThumbstickRight)
                        {
                            leftThumbstick.X = currentGamepadState.ThumbSticks.Left.X;
                            mapping.Value.Invoke(leftThumbstick);
                        } else if (button.Value == Buttons.LeftThumbstickUp || button.Value == Buttons.LeftThumbstickDown)
                        {
                            leftThumbstick.Y = currentGamepadState.ThumbSticks.Left.Y;
                            mapping.Value.Invoke(leftThumbstick);
                        }
                        else if (button.Value == Buttons.RightThumbstickLeft || button.Value == Buttons.RightThumbstickRight)
                        {
                            rightThumbStick.X = currentGamepadState.ThumbSticks.Right.X;
                            mapping.Value.Invoke(rightThumbStick);
                        }
                        else if (button.Value == Buttons.RightThumbstickUp || button.Value == Buttons.RightThumbstickDown)
                        {
                            rightThumbStick.Y = currentGamepadState.ThumbSticks.Right.Y;
                            mapping.Value.Invoke(rightThumbStick);
                        }
                        else
                        {
                            mapping.Value.Invoke(Vector2.Zero);
                        }


                    }
                    else
                    {
                        if (pressedButtons[button.Value] && buttonReleaseActions.ContainsKey(button.Value))
                        {
                            buttonReleaseActions[button.Value].Invoke();
                        }
                        pressedButtons[button.Value] = false;
                    }
                }
                
            }

            prevKeyboardState = currentKeyboardState;
            prevGamepadState = currentGamepadState;

            if (mouseState.ScrollWheelValue > prevMouseScrollWheelValue)
            {
                if (mouseWheelUpAction != null)
                {
                    mouseWheelUpAction.Invoke();
                    prevMouseScrollWheelValue = mouseState.ScrollWheelValue;
                }
            } else if (mouseState.ScrollWheelValue < prevMouseScrollWheelValue)
            {
                if (mouseWheelDownAction != null)
                {
                    mouseWheelDownAction.Invoke();
                    prevMouseScrollWheelValue = mouseState.ScrollWheelValue;
                }
            }
        }

        private class KeyMapping
        {
            public Keys? Key;
            public Buttons? Button;
            public bool SinglePressOnly;

            public KeyMapping(Keys? key, Buttons? button, bool singlePressOnly = false)
            {
                Key = key;
                this.Button = button;
                SinglePressOnly = singlePressOnly;
            }
            public override bool Equals(object obj)
            {
                return obj is KeyMapping mapping &&
                       Key == mapping.Key &&
                       Button == mapping.Button &&
                       SinglePressOnly == mapping.SinglePressOnly;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Button, SinglePressOnly);
            }
        }
    }
}
