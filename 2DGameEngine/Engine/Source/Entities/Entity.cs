using System;
using System.Collections.Generic;
using System.Text;
using GameEngine2D.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using GameEngine2D.Global;
using GameEngine2D.Util;
using GameEngine2D.Source;
using GameEngine2D.Source.Util;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Entities.Animation;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Physics.Raycast;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Engine.Source.Entities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameEngine2D.Entities
{
    public class Entity : GameObject, Interfaces.IDrawable, IUpdatable, ICollider, IRayBlocker
    {

        private readonly string DESTROY_AMINATION = "Destroy";

        private HashSet<string> tags = new HashSet<string>();

        private List<string> movementBlockerTags = new List<string>()
        {
            "Collider",
            "SlideWall",
            "Platform"
        };

        protected static OnePointCollider CollisionChecker { get; } = new OnePointCollider();

        protected Vector2 StartPosition;
        private Vector2 position;

        public bool Visible = true;
        public bool Active = true;

        protected Vector2 DrawOffset { get; set; } = Vector2.Zero;

        public Vector2 Pivot = Vector2.Zero;

        public Rectangle SourceRectangle;

        public bool BlocksMovement = false;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public float X
        {
            get => position.X;
            set => position.X = value;
        }

        public float Y
        {
            get => position.Y;
            set => position.Y = value;
        }

        protected Texture2D Sprite { get; set; }
        private HashSet<Entity> children;
        private HashSet<IUpdatable> updatables;
        private HashSet<Interfaces.IDrawable> drawables;

        private Entity parent;

        private bool hasCollisions;

        public bool HasCollision {
            get => hasCollisions;
            set { 
                hasCollisions = value; 
                if (value)
                {
                    CollisionChecker.AddObject(this);
                } else
                {
                    CollisionChecker.Remove(this);
                }
            }
        }

        protected Layer2D Layer { get; set; }
        protected List<(Vector2 start, Vector2 end)> RayBlockerLines;

        private bool blocksRay = false;

        public SoundEffect DestroySound;

        protected HashSet<GridDirection> SinglePointCollisionChecks = new HashSet<GridDirection>();
        public bool BlocksRay {
            get => blocksRay;
            set
            {
                if (!value)
                {
                    LayerManager.Instance.RayBlockersLayer.RemoveRoot(this);
                }
                else
                {
                    LayerManager.Instance.RayBlockersLayer.AddRootObject(this);
                }
                blocksRay = value;
            }
        }

#if GRAPHICS_DEBUG
        protected SpriteFont font;
#endif

        //grid coordinates
        //private float cx = 0f;
        //private float cy = 0f;
        public Vector2 GridCoordinates;

        //between 0 and 1: where the object is inside the grid cell
        //private float xr = 0.5f;
        //private float yr = 1.0f;
        protected Vector2 InCellLocation;

        //private float dx = 0;
        //private float dy = 0;
        protected Vector2 Direction = Vector2.Zero;

        protected AnimationStateMachine Animations { get; set; }

        public Ray2DEmitter RayEmitter { private get; set; }

        private Texture2D pivotMarker;

        private Dictionary<Entity, bool> collidesWith = new Dictionary<Entity, bool>();

        public float Depth = 0f;

        //public static ResolutionIndependentRenderer ResolutionIndependentRenderer;
        //public static Camera2D Camera2D;

        public Entity(Layer2D layer, Entity parent, Vector2 startPosition, Texture2D sprite = null, bool collider = false, SpriteFont font = null)
        {
            this.Layer = layer;
            GridCoordinates = CalculateGridCoord(startPosition);
            children = new HashSet<Entity>();
            updatables = new HashSet<IUpdatable>();
            drawables = new HashSet<Interfaces.IDrawable>();
            this.HasCollision = collider;
            SetSprite(sprite);
            if (parent != null) {
                this.parent = parent;
                this.parent.AddChild(this);
                this.StartPosition = this.Position = startPosition;
            } else
            {
                layer.AddRootObject(this);
                this.StartPosition = this.Position = startPosition;
            }
            //this.startPosition = this.currentPosition = startPosition;

#if GRAPHICS_DEBUG
            this.font = font;
            pivotMarker = SpriteUtil.CreateCircle(5, Color.Orange);
#endif
        }

        protected virtual void SetRayBlockers()
        {
            RayBlockerLines.Add((Position, new Vector2(Position.X, Position.Y + Config.GRID))); //0, 1
            RayBlockerLines.Add((Position, new Vector2(Position.X + Config.GRID, Position.Y))); //1, 0
            RayBlockerLines.Add((new Vector2(Position.X + Config.GRID, Position.Y), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID))); //1
            RayBlockerLines.Add((new Vector2(Position.X, Position.Y + Config.GRID), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID)));
        }

        public virtual void PreDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if (!Visible)
            {
                return;
            }

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PreDraw(spriteBatch, gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if(!Visible)
            {
                return;
            }

            Vector2 position;
            if (parent != null)
            {
                position = StartPosition + parent.GetPositionWithParent();
            } else
            {
                
                position = Position;
            }

            if (Sprite != null)
            {
                DrawSprite(spriteBatch, position);
            }
            else if (Animations != null)
            {
                Animations.Draw(spriteBatch, gameTime);
            }
#if GRAPHICS_DEBUG
            if (font != null)
            {
                if (parent != null)
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, StartPosition + parent.GetPositionWithParent(), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, Position, Color.White);
                }

            }
            spriteBatch.Draw(pivotMarker, position, Color.White);
#endif

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.Draw(spriteBatch, gameTime);
            }
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if (!Visible)
            {
                return;
            }

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PostDraw(spriteBatch, gameTime);
            }
        }

        protected virtual void DrawSprite(SpriteBatch spriteBatch, Vector2 position)
        {
            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite, (position + DrawOffset) /** new Vector2(Config.SCALE, Config.SCALE)*/, SourceRectangle, Color.White, 0f, Pivot, 1f, SpriteEffects.None, Depth);
            }
        }

        protected virtual void OnCollisionStart(Entity otherCollider)
        {

        }

        protected virtual void OnCollisionEnd(Entity otherCollider)
        {

        }

        public Entity GetSamePositionCollider()
        {
            return CollisionChecker.GetColliderAt(GridCoordinates);
        }

        public Entity GetLeftCollider()
        {
            return CollisionChecker.GetColliderAt(GridUtil.GetLeftGrid(GridCoordinates));
        }

        public Entity GetRightCollider()
        {
            return CollisionChecker.GetColliderAt(GridUtil.GetRightGrid(GridCoordinates));
        }

        public Entity GetTopCollider()
        {
            return CollisionChecker.GetColliderAt(GridUtil.GetUpperGrid(GridCoordinates));
        }

        public Entity GetBottomCollider()
        {
            return CollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates));
        }

        public bool IsIdle()
        {
            return MathUtil.SmallerEqualAbs(Direction, new Vector2(0.5f, 0.5f));
        }

        public virtual void PreUpdate(GameTime gameTime)
        {

            if (!Active)
            {
                return;
            }

            CheckCollisions();

            foreach (IUpdatable child in updatables)
            {
                child.PreUpdate(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {

            if (!Active)
            {
                return;
            }

            if (Animations != null)
            {
                Animations.Update(gameTime);
            }

            foreach (IUpdatable child in updatables)
            {
                child.Update(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {

            if (!Active)
            {
                return;
            }

            if (RayEmitter != null)
            {
                RayEmitter.UpdateRays();
            }

            foreach (IUpdatable child in updatables)
            {
                child.PostUpdate(gameTime);
            }
        }

        private void CheckCollisions()
        {

            if (SinglePointCollisionChecks.Count == 0)
            {
                return;
            }

            GridCoordinates = CalculateGridCoord();

            foreach (Entity e in new List<Entity>(collidesWith.Keys))
            {
                collidesWith[e] = false;
                //collidesWith[e] = false;
            }

            if (CollisionChecker.HasBlockingColliderAt(GridCoordinates))
            {
                if (!collidesWith.ContainsKey(GetSamePositionCollider()))
                {
                    collidesWith.Add(GetSamePositionCollider(), true);
                    OnCollisionStart(GetSamePositionCollider());
                }
                else
                {
                    collidesWith[GetSamePositionCollider()] = true;
                }
            }

            /*foreach (FaceDirection dir in SinglePointCollisionChecks)
            {
                if (dir == FaceDirection.LEFT)
                {
                    if (Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates)))
                    {
                        if (!collidesWith.ContainsKey(GetLeftCollider()))
                        {
                            collidesWith.Add(GetLeftCollider(), true);
                            OnCollisionStart(GetLeftCollider());
                        }
                        else
                        {
                            collidesWith[GetLeftCollider()] = true;
                        }
                    }
                }
            }*/

            foreach (KeyValuePair<Entity, bool> e in collidesWith.Where(e => !e.Value))
            {
                collidesWith.Remove(e.Key);
                OnCollisionEnd(e.Key);
            }
        }

        public HashSet<Entity> GetAllChildren()
        {
            return children;
        }

        public void AddChild(Entity gameObject)
        {
            children.Add(gameObject);
            if (gameObject is Interfaces.IDrawable)
            {
                drawables.Add(gameObject);
            }
            if (gameObject is IUpdatable)
            {
                updatables.Add(gameObject);
            }
        }

        public void RemoveChild(Entity gameObject)
        {
            children.Remove(gameObject);
            if (gameObject is Interfaces.IDrawable)
            {
                drawables.Remove(gameObject);
            }
            if (gameObject is IUpdatable)
            {
                updatables.Remove(gameObject);
            }
        }

        public override void Destroy()
        {
            //Visible = false;
            if (DestroySound != null)
            {
                DestroySound.Play();
            }
            if (Animations != null && Animations.HasAnimation(DESTROY_AMINATION))
            {
                Sprite = null;
                Animations.PlayAnimation(DESTROY_AMINATION);
                return;
            }
            RemoveCollisions();
            Cleanup();
        }

        protected void Cleanup()
        {
            if (Sprite != null)
            {
                Sprite.Dispose();
            }
            if (DestroySound != null)
            {
                //DestroySound.Dispose();
            }
            Layer.RemoveRoot(this);
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
            if (children.Any())
            {
                foreach (Entity o in children)
                {
                    if (o != null)
                    {
                        o.Destroy();
                    }
                }
            }
        }

        private void RemoveCollisions()
        {
            HasCollision = false;
        }

        public void SetDestroyAnimation(AbstractAnimation destroyAnimation)
        {
            if (Animations == null)
            {
                Animations = new AnimationStateMachine();
            }
            if (destroyAnimation.StartedCallback == null)
            {
                destroyAnimation.StartedCallback = () => RemoveCollisions();
                //destroyAnimation.StoppedAction = () => { };
            }
            if (destroyAnimation.StoppedCallback == null)
            {
                destroyAnimation.StoppedCallback = () => Cleanup();
                //destroyAnimation.StoppedAction = () => { };
            }
            destroyAnimation.Looping = false;
            Animations.RegisterAnimation(DESTROY_AMINATION, destroyAnimation, () => false);
        }

        public void SetSprite(Texture2D sprite)
        {
            if (sprite == null)
            {
                return;
            }

            this.Sprite = sprite;
            SourceRectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }

        public Vector2 GetPositionWithParent()
        {
            if (parent != null)
            {
                return Position + parent.GetPositionWithParent();
            }
            else
            {
                return Position;
            }
        }

        protected Vector2 CalculateGridCoord()
        {
            return CalculateGridCoord(Position);
        }

        protected Vector2 CalculateGridCoord(Vector2 position)
        {
            return new Vector2((int)Math.Floor(position.X / Config.GRID), (int)Math.Floor(position.Y / Config.GRID));
        }

        public void AddParent(Entity newParent)
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
                parent.AddChild(newParent);
                newParent.AddChild(this);
                parent = newParent;
            } else
            {
                Layer.RemoveRoot(this);
                newParent.AddChild(this);
                parent = newParent;
            }
        }

        public virtual List<(Vector2 start, Vector2 end)> GetRayBlockerLines()
        {
            if (RayBlockerLines == null)
            {
                RayBlockerLines = new List<(Vector2 start, Vector2 end)>();
                SetRayBlockers();
            }
            return RayBlockerLines;
        }

        public void SetTag(string tag)
        {
            tags.Add(tag);
            if (movementBlockerTags.Contains(tag))
            {
                BlocksMovement = true;
            }
        }

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
            foreach (string t in tags)
            {
                if (movementBlockerTags.Contains(t))
                {
                    return;
                }
            }
            BlocksMovement = false;
        }
    }
}
