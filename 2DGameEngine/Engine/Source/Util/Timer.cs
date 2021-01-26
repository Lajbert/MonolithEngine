using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Util
{
    public class Timer
    {

        private static Dictionary<Action, float> timer = new Dictionary<Action, float>();

        public static void TriggerAfter(float delayInMs, Action action, bool overrideExisting = true)
        {
            if (overrideExisting)
            {
                timer[action] = delayInMs;
            } else
            {
                if (!timer.ContainsKey(action))
                {
                    timer[action] = delayInMs;
                }
            }
        }

        public static void Update(float elapsedTime)
        {
            if (timer.Count == 0)
            {
                return;
            }

            //TODO: check the performance improvement possibilities for this
            foreach (Action key in new List<Action>(timer.Keys))
            {
                if (timer[key] - elapsedTime <= 0)
                {
                    key.Invoke();
                    timer.Remove(key);
                }
                else
                {
                    timer[key] = timer[key] - elapsedTime;
                }
            }
        }
    }
}
