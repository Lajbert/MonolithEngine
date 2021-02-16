using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities
{
    public abstract class GameObject : IGameObject
    {
        private static int GLOBAL_ID = 0;
        private int ID { get; set; } = 0 ;

        public GameObject()
        {
            ID = GLOBAL_ID++;
        }

        public abstract void Destroy();

        public override bool Equals(object obj)
        {
            if (!(obj is GameObject))
            {
                return false;
            }
            return ID == ((GameObject)obj).ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static int GetObjectCount()
        {
            return GLOBAL_ID;
        }

        public abstract Vector2 GetPosition();
        public abstract ICollection<string> GetTags();
    }
}
