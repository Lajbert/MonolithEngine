using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonolithEngine
{
    /// <summary>
    /// Class to represent static game entities.
    /// Objects created from this class has no Velocity.
    /// </summary>
    public class Entity : GameObject, IColliderEntity, IRayBlocker
    {

        protected List<IGameObject> Children = new List<IGameObject>();
        private List<IGameObject> childrenToAdd = new List<IGameObject>();
        private List<IGameObject> childrenToRemove = new List<IGameObject>();

        public AbstractScene Scene;

        internal bool IsCollisionCheckedInCurrentLoop = false;

        public bool IsDestroyed { 
            get {
                return Destroyed || BeingDestroyed;
            } 
        }

        public Vector2 DrawPosition;

        protected float CollisionOffsetLeft = 0f;
        protected float CollisionOffsetRight = 0f;
        protected float CollisionOffsetBottom = 0f;
        protected float CollisionOffsetTop = 0f;

        private ComponentList componentList;

        private Dictionary<Type, bool> collidesAgainst = new Dictionary<Type, bool>();
        private HashSet<Type> triggeredAgainst = new HashSet<Type>();

        private bool checkGridCollisions = false;

        public bool CheckGridCollisions
        {
            get => checkGridCollisions;
            set { 
                if ( value != checkGridCollisions )
                {
                    checkGridCollisions = value;
                    Scene.CollisionEngine.ObjectChanged(this);
                }
            }
        }

        private bool canFireTriggers = false;
        public bool CanFireTriggers
        {
            get => canFireTriggers;

            set
            {
                canFireTriggers = value;
                Scene.CollisionEngine.ObjectChanged(this);
            }
        }

        public Direction CurrentFaceDirection { get; set; } = Direction.CENTER;

        private bool visible = true;
        private bool active = true;

        public bool Visible {
            get => visible;
            set
            {
                if (value != visible)
                {
                    Layer.OnObjectChanged(this);
                }
                visible = value;
            }
        }
        public bool Active
        {
            get => active;
            set
            {
                if (value != active)
                {
                    Layer.OnObjectChanged(this);
                }
                active = value;
            }
        }

        private float drawPriority = 0;

        public float DrawPriority {
            get => drawPriority;

            set
            {
                if (value != drawPriority)
                {
                    drawPriority = value;
                    Layer.SortByPriority();
                }
            }
        }

        public Vector2 Pivot = Vector2.Zero;

        protected Layer Layer { get; set; }
        protected List<(Vector2 start, Vector2 end)> RayBlockerLines;

        public bool BlocksRay { get; set; }

#if DEBUG
        public Func<string> DebugFunction = null;
#endif

        //protected Ray2DEmitter RayEmitter { get; set; }

        private Texture2D pivotMarker;

        public float Depth = 0f;

#if DEBUG
        public bool DEBUG_SHOW_PIVOT = false;

        public bool DEBUG_SHOW_COLLIDER = false;

        public bool DEBUG_SHOW_RAYCAST = false;
#endif

        protected bool Destroyed = false;
        protected bool BeingDestroyed = false;

        public bool CollisionsEnabled { get; set; } = true;

        public float DrawRotation;

        public sealed override IGameObject Parent
        {
            get => base.Parent;

            set
            {
                base.Parent = value;
                Layer.OnObjectChanged(this);
            }
        }

        private UserInputController UserInput;

        public Entity(Layer layer, Entity parent = null, Vector2 startPosition = default) : base()
        {
            componentList = new ComponentList(this);
            DrawPosition = startPosition;
            Transform = new StaticTransform(this, startPosition);
            Layer = layer;
            Scene = layer.Scene;
            layer.OnObjectChanged(this);
            Parent = parent;
        }

        protected virtual void SetRayBlockers()
        {
            RayBlockerLines.Clear();
            RayBlockerLines.Add((Transform.Position, new Vector2(Transform.X, Transform.Y + Config.GRID))); //0, 1
            RayBlockerLines.Add((Transform.Position, new Vector2(Transform.X + Config.GRID, Transform.Y))); //1, 0
            RayBlockerLines.Add((new Vector2(Transform.X + Config.GRID, Transform.Y), new Vector2(Transform.X + Config.GRID, Transform.Y + Config.GRID))); //1
            RayBlockerLines.Add((new Vector2(Transform.X, Transform.Y + Config.GRID), new Vector2(Transform.X + Config.GRID, Transform.Y + Config.GRID)));
        }

        public T GetComponent<T>() where T : IComponent
        {
            return componentList.GetComponent<T>();
        }

        public List<T> GetComponents<T>() where T : IComponent
        {
            return componentList.GetComponents<T>();
        }

        public void AddComponent<T>(T newComponent) where T : IComponent
        {

            componentList.AddComponent(newComponent);

            if (newComponent is ICollisionComponent || newComponent is ITrigger)
            {
                Scene.CollisionEngine.ObjectChanged(this);
            }

            if (newComponent is UserInputController)
            {
                UserInput = newComponent as UserInputController;
            }
        }

        public void RemoveComponent<T>(T component) where T : IComponent
        {
            if (component is ICollisionComponent || component is ITrigger)
            {
                Scene.CollisionEngine.ObjectChanged(this);
            }
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            componentList.RemoveComponent<T>();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            componentList.DrawAll(spriteBatch);

#if DEBUG
            if (DEBUG_SHOW_PIVOT)
            {
                if (pivotMarker == null)
                {
                    pivotMarker = AssetUtil.CreateCircle(5, Color.Red, true);
                }
                spriteBatch.Draw(pivotMarker, Transform.Position - new Vector2(2.5f, 2.5f), Color.White);
            }
            if (DebugFunction != null)
            {
                spriteBatch.DrawString(Assets.GetFont("DebugFont"), DebugFunction.Invoke(), DrawPosition, Color.Red);
            }
#endif
            /*if (font != null)
            {
                spriteBatch.DrawString(font, "InCell : " + Transform.InCellLocation, DrawPosition, Color.Red);
            }*/

            if (Children.Count > 0)
            {
                foreach (Entity child in Children)
                {
                    if (child.Visible)
                    {
                        child.Draw(spriteBatch);
                    }
                }
            }

            HandleChangedChildrenChildren();
            
        }

        public virtual void PreUpdate()
        {
            componentList.PreUpdateAll();

            foreach (Entity child in Children)
            {
                if (child.Active)
                {
                    child.PreUpdate();
                }
            }
        }

        public virtual void PreFixedUpdate()
        {
            foreach (Entity child in Children)
            {
                if (child.Active)
                {
                    child.PreFixedUpdate();
                }
            }

            HandleChangedChildrenChildren();
        }

        public virtual void FixedUpdate()
        {
            //float gameTime = (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET;
            //previousPosition = DrawPosition;
            /*previousPosition = Transform.Position;
            previousPosition.X = (int)previousPosition.X;
            previousPosition.Y = (int)previousPosition.Y;*/

            if (!IsCollisionCheckedInCurrentLoop)
            {
                Scene.CollisionEngine.CheckCollisions(this);
            }

            componentList.UpdateAll();

            foreach (Entity child in Children)
            {
                if (child.Active)
                {
                    child.FixedUpdate();
                }
            }

            HandleChangedChildrenChildren();
        }

        public virtual void Update()
        {
            foreach (Entity child in Children)
            {
                if (child.Active)
                {
                    child.Update();
                }
            }

            HandleChangedChildrenChildren();
        }

        public virtual void PostUpdate()
        {
            /*if (RayEmitter != null)
            {
                RayEmitter.UpdateRays();
            }*/

            componentList.PostUpdateAll();

            foreach (Entity child in Children)
            {
                if (child.Active)
                {
                    child.PostUpdate();
                }
            }

            HandleChangedChildrenChildren();
        }

        public override void Destroy()
        {
            if (BeingDestroyed || Destroyed)
            {
                return;
            }
            BeingDestroyed = true;

            RemoveCollisions();

            ClearAllComponents();

            Cleanup();

            Destroyed = true;
            BeingDestroyed = false;
        }

        protected void Cleanup()
        {
            componentList.ClearAll();

            if (Parent != null)
            {
                Parent.RemoveChild(this);
            }
            if (Children.Any())
            {
                foreach (Entity o in Children.ToList())
                {
                    if (o != null)
                    {
                        o.Destroy();
                    }
                }
            }
            Active = false;
            Visible = false;
        }

        internal void ClearAllComponents()
        {
            componentList.ClearAll();
        }

        protected virtual void RemoveCollisions()
        {
            componentList.Clear<ICollisionComponent>();
            componentList.Clear<ITrigger>();
            collidesAgainst.Clear();
            CanFireTriggers = false;
            CollisionsEnabled = false;
            Scene.CollisionEngine.ObjectChanged(this);
            //RayEmitter = null;
            BlocksRay = false;
        }

        public void SetSprite(Texture2D sprite)
        {
            if (sprite == null)
            {
                return;
            }
            AddComponent(new Sprite(this, sprite, new Rectangle(0, 0, sprite.Width, sprite.Height)));
        }

        public virtual List<(Vector2 start, Vector2 end)> GetRayBlockerLines()
        {
            if (RayBlockerLines == null)
            {
                RayBlockerLines = new List<(Vector2 start, Vector2 end)>();
            }
            SetRayBlockers();
            return RayBlockerLines;
        }

        public Vector2 GetGridCoord()
        {
            return Transform.GridCoordinates;
        }

        public float GetCollisionOffset(Direction direction)
        {
            if (direction == Direction.EAST)
            {
                return CollisionOffsetLeft;
            } else if (direction == Direction.WEST)
            {
                return CollisionOffsetRight;
            }
            else if (direction == Direction.NORTH)
            {
                return CollisionOffsetTop;
            }
            else if (direction == Direction.SOUTH)
            {
                return CollisionOffsetBottom;
            }
            throw new Exception("Unsupported direction");
        }

        public ICollisionComponent GetCollisionComponent()
        {
            return componentList.GetComponent<ICollisionComponent>();
        }

        public virtual void OnCollisionStart(IGameObject otherCollider)
        {

        }

        public virtual void OnCollisionEnd(IGameObject otherCollider)
        {

        }

        internal virtual void HandleCollisionStart(IGameObject otherCollider, bool allowOverlap)
        {
            OnCollisionStart(otherCollider);
        }

        internal virtual void HandleCollisionEnd(IGameObject otherCollider)
        {
            OnCollisionEnd(otherCollider);
        }

        void IColliderEntity.CollisionStarted(IGameObject otherCollider, bool allowOverlap)
        {
            HandleCollisionStart(otherCollider, allowOverlap);
        }

        void IColliderEntity.CollisionEnded(IGameObject otherCollider)
        {
            HandleCollisionEnd(otherCollider);
        }

        /*internal virtual void CollisionStarted(IGameObject otherCollider, bool allowOverlap)
        {
            OnCollisionStart(otherCollider);
        }

        internal virtual void CollisionEnded(IGameObject otherCollider)
        {
            OnCollisionEnd(otherCollider);
        }*/

        Dictionary<Type, bool> IColliderEntity.GetCollidesAgainst()
        {
            return collidesAgainst;
        }

        HashSet<Type> IColliderEntity.GetTriggeredAgainst()
        {
            return triggeredAgainst;
        }

        public void AddCollisionAgainst(Type type, bool allowOverlap = true)
        {
            collidesAgainst[type] = allowOverlap;
            Scene.CollisionEngine.ObjectChanged(this);
        }

        public void AddTriggeredAgainst(Type type, bool allowOverlap = true)
        {
            triggeredAgainst.Add(type);
            Scene.CollisionEngine.ObjectChanged(this);
        }

        /*public void RemoveCollisionAgainst(Type type)
        {
            collidesAgainst.Remove(type);
            Scene.CollisionEngine.ObjectChanged(this);
        }

        public void RemoveTriggeredAgainst(Type type)
        {
            triggeredAgainst.Remove(type);
            Scene.CollisionEngine.ObjectChanged(this);
        }*/

        public ICollection<ITrigger> GetTriggers()
        {
            return componentList.GetComponents<ITrigger>();
        }

        public void RemoveTrigger(AbstractTrigger trigger)
        {
            foreach (ITrigger t in componentList.GetComponents<ITrigger>())
            {
                if (t.Equals(trigger))
                {
                    RemoveTrigger(t);
                    return;
                }
            }
        }

        public void RemoveTrigger(ITrigger trigger)
        {
            componentList.RemoveComponent(trigger);
            Scene.CollisionEngine.ObjectChanged(this);
        }

        public virtual void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
        }

        public virtual void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
        }

        public override void AddTag(string tag)
        {
            base.AddTag(tag);
        }

        public override void RemoveTag(string tag)
        {
            base.RemoveTag(tag);
        }

        public override bool IsAlive()
        {
            return !IsDestroyed && !BeingDestroyed;
        }

        public override void AddChild(IGameObject gameObject)
        {
            childrenToAdd.Add(gameObject);
        }

        public override void RemoveChild(IGameObject gameObject)
        {
            childrenToRemove.Add(gameObject);
        }

        private void HandleChangedChildrenChildren()
        {
            if (childrenToAdd.Count > 0)
            {
                foreach (IGameObject child in childrenToAdd)
                {
                    Children.Add(child);
                }

                childrenToAdd.Clear();
            }

            if (childrenToRemove.Count > 0)
            {
                foreach (IGameObject child in childrenToRemove)
                {
                    Children.Remove(child);
                }

                childrenToRemove.Clear();
            }
        }
    }
}
