using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Controller
{
    public class UserInputController
    {

        private Dictionary<Keys, bool> pressedKeys = new Dictionary<Keys, bool>();
        private Dictionary<KeyMapping, Action> keyActions = new Dictionary<KeyMapping, Action>();
        private KeyboardState currentKeyboardState;
        private KeyboardState prevKeyboardState;
        private MouseState mouseState;

        private int prevMouseScrollWheelValue = 0;
        private Action mouseWheelUpAction;
        private Action mouseWheelDownAction;

        public void RegisterControllerState(Keys key, Action action, bool singlePressOnly = false) {
            keyActions.Add(new KeyMapping(key, singlePressOnly), action);
            if (!pressedKeys.ContainsKey(key)) {
                pressedKeys.Add(key, false);
            }
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

            foreach (KeyValuePair<KeyMapping, Action> mapping in keyActions)
            {
                Keys key = mapping.Key.Key;
                if (currentKeyboardState.IsKeyDown(key))
                {
                    if(mapping.Key.SinglePressOnly && (prevKeyboardState != null && (prevKeyboardState == currentKeyboardState || pressedKeys[key] ))) {
                        continue;
                    }
                    pressedKeys[key] = true;
                    mapping.Value.Invoke();
                } else
                {
                    pressedKeys[key] = false;
                }
            }

            prevKeyboardState = currentKeyboardState;

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
            public Keys Key;
            public bool SinglePressOnly;

            public KeyMapping(Keys key, bool singlePressOnly)
            {
                Key = key;
                SinglePressOnly = singlePressOnly;
            }

            public override bool Equals(object obj)
            {
                return obj is KeyMapping mapping &&
                       Key == mapping.Key &&
                       SinglePressOnly == mapping.SinglePressOnly;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, SinglePressOnly);
            }
        }
    }
}
