using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Util
{
    public class Timer
    {

        private static Dictionary<Action, float> triggeredAction = new Dictionary<Action, float>();
        private static Dictionary<string, float> timers = new Dictionary<string, float>();

        public static void TriggerAfter(float delayInMs, Action action, bool overrideExisting = true)
        {
            if (overrideExisting)
            {
                triggeredAction[action] = delayInMs;
            } else
            {
                if (!triggeredAction.ContainsKey(action))
                {
                    triggeredAction[action] = delayInMs;
                }
            }
        }

        public static void SetTimer(string name, float duration, bool overrideExisting = true)
        {
            if (overrideExisting)
            {
                timers[name] = duration;
            }
            else
            {
                if (!timers.ContainsKey(name))
                {
                    timers[name] = duration;
                }
            }
        }

        public static void Update(float elapsedTime)
        {
            if (triggeredAction.Count == 0 && timers.Count == 0)
            {
                return;
            }

            //TODO: check the performance improvement possibilities for this
            foreach (Action key in new List<Action>(triggeredAction.Keys))
            {
                if (triggeredAction[key] - elapsedTime <= 0)
                {
                    key.Invoke();
                    triggeredAction.Remove(key);
                }
                else
                {
                    triggeredAction[key] -= elapsedTime;
                }
            }

            foreach (string key in new List<string>(timers.Keys))
            {
                if (timers[key] - elapsedTime <= 0)
                {
                    timers.Remove(key);
                }
                else
                {
                    timers[key] -= elapsedTime;
                }
            }
        }

        public static bool IsSet(string timer)
        {
            return timers.ContainsKey(timer);
        }
    }
}
