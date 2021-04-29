using System.Collections.Generic;

namespace MonolithEngine
{
    public abstract class GameObject : IGameObject
    {
        private static int GLOBAL_ID = 0;
        private int ID { get; set; } = 0 ;

        protected HashSet<IGameObject> Children;

        protected HashSet<string> Tags = new HashSet<string>();

        public AbstractTransform Transform { get; set; }

        private IGameObject parent;
        public virtual IGameObject Parent
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

        public GameObject(IGameObject parent = null)
        {
            if (parent != null)
            {
                Parent = parent;
            }
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

        public int GetID()
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

        public ICollection<string> GetTags()
        {
            return Tags;
        }

        public virtual void AddTag(string tag)
        {
            Tags.Add(tag);
        }

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public bool HasAnyTag(ICollection<string> tags)
        {
            foreach (string tag in tags)
            {
                if (tags.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

        public abstract bool IsAlive();
    }
}
