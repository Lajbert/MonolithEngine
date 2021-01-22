using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Util
{
    public class Timer
    {

        private static Dictionary<string, float> timer = new Dictionary<string, float>();

        public static void StartTimer(string name, float time)
        {
            timer[name] = time;
        }

        public static void Update(float elapsedTime)
        {
            if (timer.Count == 0)
            {
                return;
            }

            //TODO: check the performance improvement possibilities for this
            foreach (string key in new List<string>(timer.Keys))
            {
                if (timer[key] - elapsedTime <= 0)
                {
                    timer.Remove(key);
                }
                else
                {
                    timer[key] = timer[key] - elapsedTime;
                }
            }
        }

        public static bool IsCounting(string name)
        {
            return timer.ContainsKey(name);
        }
    }
}
