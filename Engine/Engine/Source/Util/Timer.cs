using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MonolithEngine.Engine.Source.Global;

namespace MonolithEngine.Engine.Source.Util
{
    public class Timer
    {

        private static Dictionary<Action, float> triggeredActions = new Dictionary<Action, float>();
        private static List<RepeatedAction> repeatedActions = new List<RepeatedAction>();
        private static Dictionary<string, float> timers = new Dictionary<string, float>();
        private static List<LerpAction> lerps = new List<LerpAction>();

        public static void TriggerAfter(float delayInMs, Action action, bool overrideExisting = true)
        {
            if (overrideExisting)
            {
                triggeredActions[action] = delayInMs;
            } else
            {
                if (!triggeredActions.ContainsKey(action))
                {
                    triggeredActions[action] = delayInMs;
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

            //TODO: check the performance improvement possibilities for this
            foreach (Action key in triggeredActions.Keys.ToList())
            {
                if (triggeredActions[key] - elapsedTime <= 0)
                {
                    key.Invoke();
                    triggeredActions.Remove(key);
                }
                else
                {
                    triggeredActions[key] -= elapsedTime;
                }
            }

            foreach (string key in timers.Keys.ToList())
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

            foreach (LerpAction lerp in lerps.ToList())
            {
                if (lerp.progress <= lerp.duration)
                {
                    lerp.callback.Invoke(MathHelper.Lerp(lerp.from, lerp.to, lerp.progress / lerp.duration));
                }
                else
                {
                    lerps.Remove(lerp);
                }
                lerp.progress += elapsedTime;
            }

            foreach (RepeatedAction action in repeatedActions.ToList())
            {
                if (action.progress <= action.duration)
                {
                    action.action.Invoke(elapsedTime);
                    action.progress += elapsedTime;
                } else
                {
                    repeatedActions.Remove(action);
                }
            }

        }

        public static void Clear()
        {
            triggeredActions.Clear();
            repeatedActions.Clear();
            timers.Clear();
            lerps.Clear();
        }

        public static bool IsSet(string timer)
        {
            return timers.ContainsKey(timer);
        }

        public static void CancelAction(Action action)
        {
            triggeredActions.RemoveIfExists(action);
        }

        public static void Repeat(float duration, Action<float> action)
        {
            repeatedActions.Add(new RepeatedAction(0, duration, action));
        }

        public static void Lerp(float duration, float from, float to, Action<float> callback)
        {
            lerps.Add(new LerpAction(0, duration, from, to, callback));
        }

        private class RepeatedAction
        {
            public float progress;
            public float duration;
            public Action<float> action;

            public RepeatedAction(float progress, float duration, Action<float> action)
            {
                this.progress = progress;
                this.action = action;
                this.duration = duration;
            }
        }

        private class LerpAction
        {
            public Action<float> callback;
            public float progress;
            public float duration;
            public float from;
            public float to;

            public LerpAction(float progress, float endTime, float from, float to, Action<float> callback)
            {
                this.callback = callback;
                this.progress = progress;
                this.duration = endTime;
                this.from = from;
                this.to = to;
            }
        }
    }
}
