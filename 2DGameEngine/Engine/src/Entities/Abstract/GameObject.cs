using GameEngine2D.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities
{
    abstract class GameObject
    {
        private static int GLOBAL_ID = 0;
        private int UNIQUE_ID = 0;

        public GameObject()
        {
            SetID();
        }

        public void SetID()
        {
            UNIQUE_ID = GLOBAL_ID++;
        }

        public int GetID()
        {
            return this.UNIQUE_ID;
        }

        public abstract void Destroy();

        public override bool Equals(object obj)
        {
            if (!(obj is GameObject))
            {
                return false;
            }
            return this.UNIQUE_ID == ((GameObject)obj).UNIQUE_ID;
        }

        public override int GetHashCode()
        {
            return UNIQUE_ID;
        }

    }
}
