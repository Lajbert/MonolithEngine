using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Transform;
using GameEngine2D.Global;
using GameEngine2D.Source.Util;
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

        protected HashSet<IGameObject> Children;

        public AbstractTransform Transform { get; set; }

        private IGameObject parent;
        public IGameObject Parent
        {
            get => parent;

            set {
                if (value != null)
                {
                    //Transform.Position = position;
                    if (parent != null)
                    {
                        parent.RemoveChild(this);
                    }
                    parent = value;
                    value.AddChild(this);
                }
                else
                {
                    if (parent != null)
                    {
                        Transform.DetachFromParent();
                        //Transform.GridCoordinates = CalculateGridCoord();
                        parent.RemoveChild(this);
                    }
                    else
                    {
                        //Transform.Position = position;
                    }
                    parent = null;
                }
                //Transform.GridCoordinates = CalculateGridCoord();
            }
        }

        public GameObject(IGameObject parent)
        {
            Parent = parent;
            ID = GLOBAL_ID++;
            Children = new HashSet<IGameObject>();
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

        public void AddChild(IGameObject gameObject)
        {
            Children.Add(gameObject);
        }

        public void RemoveChild(IGameObject gameObject)
        {
            Children.Remove(gameObject);
        }

        public abstract ICollection<string> GetTags();
    }
}
