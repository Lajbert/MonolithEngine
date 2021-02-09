using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Physics.Raycast;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities.Animation;
using GameEngine2D.Source.GridCollision;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine2D.Entities
{
    public class Entity : GameObject, IRayBlocker, IGridCollider
    {

        protected float CollisionOffsetLeft = 0f;
        protected float CollisionOffsetRight = 0f;
        protected float CollisionOffsetBottom = 0f;
        protected float CollisionOffsetTop = 0f;

        private readonly string DESTROY_AMINATION = "Destroy";

        protected HashSet<string> Tags = new HashSet<string>();

        private HashSet<Direction> blockedFrom = new HashSet<Direction>();

        public Direction CurrentFaceDirection { get; set; } = Direction.CENTER;

        protected static GridCollisionChecker GridCollisionChecker { get; } = new GridCollisionChecker();

        public bool Visible = true;
        public bool Active = false;

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

        public Vector2 DrawOffset;

        public Vector2 Pivot = Vector2.Zero;

        public Rectangle SourceRectangle;

        private Vector2 position;

        public Vector2 Position
        {
            get
            {
                if (parent == null)
                {
                    return position;
                }
                return parent.Position + position;
            }
            set {
                position = value;
            }
        }

        public float X
        {
            get
            {
                return Position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public float Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                position.Y = value;
            }
        }

        protected Texture2D Sprite { get; set; }
        private HashSet<Entity> children;

        protected Entity parent;

        private bool blocksMovement = false;
        private bool onGrid = false;

        public bool BlocksMovement {
            get => blocksMovement;
            set {
                blocksMovement = value;
                if (value && !onGrid)
                {
                    GridCollisionChecker.Add(this);
                    onGrid = true;
                }
            }
        }

        protected Layer Layer { get; set; }
        protected List<(Vector2 start, Vector2 end)> RayBlockerLines;

        public bool BlocksRay { get; set; }

        public SoundEffect DestroySound;

        public HashSet<Direction> GridCollisionCheckDirections = new HashSet<Direction>();

        protected SpriteFont font;

        public Vector2 GridCoordinates = Vector2.Zero;

        //between 0 and 1: where the object is inside the grid cell
        protected Vector2 InCellLocation = new Vector2(0.5f, 1f);

        public AnimationStateMachine Animations { get; set; }

        protected Ray2DEmitter RayEmitter { get; set; }

        public ICollisionComponent CollisionComponent { get; set; }

        private Dictionary<Entity, bool> circleCollisions = new Dictionary<Entity, bool>();

        private Texture2D pivotMarker;
        private Texture2D circleColliderMarker;

        private Dictionary<(Entity, Direction), bool> collidesWithOnGrid = new Dictionary<(Entity, Direction), bool>();

        public float Depth = 0f;

        public bool DEBUG_SHOW_PIVOT = false;

        public bool DEBUG_SHOW_CIRCLE_COLLIDER = false;

        public bool DEBUG_SHOW_RAYCAST = false;

        public Vector2 DrawPosition
        {
            get {
                return Position + DrawOffset;
            }

            private set {}
        }

        protected bool Destroyed = false;
        protected bool BeingDestroyed = false;

        public Entity(Layer layer, Entity parent, Vector2 startPosition, Texture2D sprite = null, SpriteFont font = null)
        {
            this.Layer = layer;
            children = new HashSet<Entity>();
            this.font = font;
            SetParent(parent, startPosition);
            SetSprite(sprite);
        }

        public void SetParent(Entity newParent, Vector2 position)
        {
            if (newParent != null)
            {
                Position = position;
                Layer.RemoveRoot(this);
                if (parent != null)
                {
                    parent.RemoveChild(this);
                }
                parent = newParent;
                newParent.AddChild(this);
            }
            else
            {
                if (parent != null)
                {
                    this.position += parent.Position;
                    parent.RemoveChild(this);
                } else
                {
                    Position = position;
                }
                parent = null;
                Layer.AddRootObject(this);
            }
            //UpdateInCellCoord();
            GridCoordinates = CalculateGridCoord();
        }

        public void SetParent(Entity newParent)
        {
            SetParent(newParent, Vector2.Zero);
        }

        public void RemoveParent()
        {
            SetParent(null, Vector2.Zero);
        }

        protected virtual void SetRayBlockers()
        {
            RayBlockerLines.Clear();
            RayBlockerLines.Add((Position, new Vector2(Position.X, Position.Y + Config.GRID))); //0, 1
            RayBlockerLines.Add((Position, new Vector2(Position.X + Config.GRID, Position.Y))); //1, 0
            RayBlockerLines.Add((new Vector2(Position.X + Config.GRID, Position.Y), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID))); //1
            RayBlockerLines.Add((new Vector2(Position.X, Position.Y + Config.GRID), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID)));
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite, DrawPosition, SourceRectangle, Color.White, 0f, Pivot, 1f, SpriteEffects.None, Depth);
            }
            else if (Animations != null)
            {
                Animations.Draw(spriteBatch, gameTime);
            }

            if (DEBUG_SHOW_PIVOT)
            {
                if (pivotMarker == null)
                {
                    pivotMarker = SpriteUtil.CreateCircle(5, Color.Red, true);
                }
                if (font != null)
                {
                    //spriteBatch.DrawString(font, "Veloctiy.Y : " + Velocity.Y, DrawPosition, Color.Black);
                }
                spriteBatch.Draw(pivotMarker, Position - new Vector2(2.5f, 2.5f), Color.White);
            }
            
            if (DEBUG_SHOW_CIRCLE_COLLIDER)
            {
                if (circleColliderMarker == null)
                {
                    circleColliderMarker = SpriteUtil.CreateCircle((int)((CircleCollisionComponent)CollisionComponent).Radius * 2, Color.Black);
                }
                if (CollisionComponent != null)
                {
                    spriteBatch.Draw(circleColliderMarker, ((CircleCollisionComponent)CollisionComponent).Position - new Vector2(((CircleCollisionComponent)CollisionComponent).Radius, ((CircleCollisionComponent)CollisionComponent).Radius), Color.White);
                } else
                {
                    Logger.Log("Tried to print circle collider, but it's null!");
                }
            }

            /*if (font != null)
            {
                spriteBatch.DrawString(font, "InCell : " + InCellLocation, DrawPosition, Color.Red);
            }*/

            if (children.Count > 0)
            {
                foreach (Entity child in children)
                {
                    child.Draw(spriteBatch, gameTime);
                }
            }
            
        }

        protected virtual void OnGridCollisionStart(Entity otherCollider, Direction direction)
        {

        }

        protected virtual void OnGridCollisionEnd(Entity otherCollider, Direction direction)
        {

        }

        public Entity GetSamePositionCollider()
        {
            return GridCollisionChecker.GetColliderAt(GridCoordinates) as Entity;
        }

        public virtual void PreUpdate(GameTime gameTime)
        {

            UpdateCollisions();

            foreach (Entity child in children)
            {
                child.PreUpdate(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {

            if (Animations != null)
            {
                Animations.Update(gameTime);
            }

            foreach (Entity child in children.ToList())
            {
                child.Update(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {

            if (RayEmitter != null)
            {
                RayEmitter.UpdateRays();
            }

            foreach (Entity child in children)
            {
                child.PostUpdate(gameTime);
            }
        }

        private void UpdateCollisions()
        {

            if (GridCollisionCheckDirections.Count > 0)
            {

                foreach ((Entity, Direction) e in collidesWithOnGrid.Keys.ToList())
                {
                    collidesWithOnGrid[e] = false;
                }

                foreach ((Entity, Direction) collision in GridCollisionChecker.HasGridCollisionAt(this, GridCollisionCheckDirections))
                {
                    if (!collidesWithOnGrid.ContainsKey(collision))
                    {
                        OnGridCollisionStart(collision.Item1, collision.Item2);
                    }
                    collidesWithOnGrid[collision] = true;
                }

                foreach (KeyValuePair<(Entity, Direction), bool> e in collidesWithOnGrid.Where(e => !e.Value))
                {
                    collidesWithOnGrid.Remove(e.Key);
                    OnGridCollisionEnd(e.Key.Item1, e.Key.Item2);
                }
            }


            /*if (EnableCircleCollisions && CircleCollider != null)
            {

                foreach (Entity e in circleCollisions.Keys.ToList())
                {
                    circleCollisions[e] = false;
                }

                foreach (Entity e in LayerManager.Instance.EntityLayer.GetAll().ToList())
                {
                    if (CircleCollider == null && !EnableCircleCollisions)
                    {
                        break;
                    }
                    if (e.Equals(this) || (Math.Abs(Position.X - e.Position.X) > Config.GRID * 2 && Math.Abs(Position.Y - e.Position.Y) > Config.GRID * 2))
                    {
                        continue;
                    }
                    if (e.CircleCollider != null)
                    {
                        CollisionResult collResult = CircleCollider.Overlaps(e);
                        if (collResult != CollisionResult.NO_COLLISION)
                        {
                            if (!circleCollisions.ContainsKey(e))
                            {
                                OnCircleCollisionStart(e, collResult);
                            }
                            circleCollisions[e] = true;
                        }
                    }
                }

                foreach (Entity e in circleCollisions.Keys.ToList())
                {
                    if (!circleCollisions[e])
                    {
                        OnCircleCollisionEnd(e);
                        circleCollisions.Remove(e);
                    }
                }
            }   */
        }

        public HashSet<Entity> GetAllChildren()
        {
            return children;
        }

        public void AddChild(Entity gameObject)
        {
            children.Add(gameObject);
        }

        public void RemoveChild(Entity gameObject)
        {
            children.Remove(gameObject);
        }

        public override void Destroy()
        {
            if (BeingDestroyed)
            {
                return;
            }
            BeingDestroyed = true;
            RemoveCollisions();
            if (DestroySound != null)
            {
                DestroySound.Play();
            }
            if (Animations != null && Animations.HasAnimation(DESTROY_AMINATION + CurrentFaceDirection))
            {
                //RemoveCollisions();
                Animations.PlayAnimation(DESTROY_AMINATION + CurrentFaceDirection);
                //Animations.GetAnimation(DESTROY_AMINATION).StoppedCallback = () => Cleanup();
            } 
            else
            {
                Cleanup();
            }
        }

        protected void Cleanup()
        {
            if (Sprite != null)
            {
                Sprite.Dispose();
            }
            if (Animations != null)
            {
                Animations.Destroy();
                Animations = null;
            }
            if (DestroySound != null)
            {
                DestroySound.Dispose();
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
            //Active = false;
            //Visible = false;
            Destroyed = true;
        }

        protected virtual void RemoveCollisions()
        {
            CollisionComponent = null;
            BlocksMovement = false;
            RayEmitter = null;
            BlocksRay = false;
            GridCollisionCheckDirections = new HashSet<Direction>();
            if (BlocksMovement)
            {
                GridCollisionChecker.Remove(this);
            }
        }

        public void SetDestroyAnimation(AbstractAnimation destroyAnimation, Direction direction = Direction.CENTER)
        {
            if (Animations == null)
            {
                Animations = new AnimationStateMachine();
            }
            //destroyAnimation.StartedCallback += () => RemoveCollisions();
            destroyAnimation.StoppedCallback += () => Cleanup();
            destroyAnimation.Looping = false;
            Animations.RegisterAnimation(DESTROY_AMINATION + direction.ToString(), destroyAnimation, () => false);
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

        protected void UpdateInCellCoord()
        {
            InCellLocation.X = (Position.X / GridCoordinates.X) % 1;
            InCellLocation.Y = (Position.Y / GridCoordinates.Y) % 1;
        }

        protected Vector2 CalculateGridCoord(Vector2 position)
        {
            return new Vector2((int)Math.Floor(Position.X / Config.GRID), (int)Math.Floor(Position.Y / Config.GRID));
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

        public void AddTag(string tag)
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

        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

        public void AddBlockedDirection(Direction direction)
        {
            blockedFrom.Add(direction);
        }

        public void RemoveBlockedDirection(Direction direction)
        {
            blockedFrom.Remove(direction);
        }

        public bool IsBlockedFrom(Direction direction)
        {
            return blockedFrom.Count == 0 || blockedFrom.Contains(direction);
        }

        public Vector2 GetInCellLocation()
        {
            return InCellLocation;
        }

        public Vector2 GetGridCoord()
        {
            return GridCoordinates;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public float GetCollisionOffset(Direction direction)
        {
            if (direction == Direction.RIGHT)
            {
                return CollisionOffsetLeft;
            } else if (direction == Direction.LEFT)
            {
                return CollisionOffsetRight;
            }
            else if (direction == Direction.UP)
            {
                return CollisionOffsetTop;
            }
            else if (direction == Direction.DOWN)
            {
                return CollisionOffsetBottom;
            }
            throw new Exception("Unsupported direction");
        }

        public bool BlocksMovementFrom(Direction direction)
        {
            return BlocksMovement && IsBlockedFrom(direction);
        }

        public HashSet<CollisionType> GetCollisionProfile()
        {
            return new HashSet<CollisionType>() { CollisionType.CIRCLE };
        }

        public ICollection<string> GetTags()
        {
            return Tags;
        }
    }
}
