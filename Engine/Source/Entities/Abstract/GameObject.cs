using System.Collections.Generic;

namespace MonolithEngine
{

    /// <summary>
    /// Base class for all game entities.
    /// Each object has a unique ID to make debugging easier
    /// and make comparisons fast.
    /// Also, it contains parent-child relationship between objects.
    /// </summary>
    public abstract class GameObject : IGameObject
    {
        private static int GLOBAL_ID = 0;
        private int ID { get; set; } = 0 ;

        protected HashSet<string> Tags = new HashSet<string>();

        public AbstractTransform Transform { get; set; }

        private IGameObject parent;
        public virtual IGameObject Parent
        {
            get => parent;

            set {
                if (value != null)
                {
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
                        parent.RemoveChild(this);
                    }
                    parent = null;
                }
            }
        }

        public GameObject(IGameObject parent = null)
        {
            if (parent != null)
            {
                Parent = parent;
            }
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

        public int GetID()
        {
            return ID;
        }

        public static int GetObjectCount()
        {
            return GLOBAL_ID;
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
        public abstract void AddChild(IGameObject gameObject);
        public abstract void RemoveChild(IGameObject gameObject);
    }
}
