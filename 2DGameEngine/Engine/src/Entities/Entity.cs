using System;
using System.Collections.Generic;
using System.Text;
using GameEngine2D.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using GameEngine2D.Global;
using GameEngine2D.Util;
using GameEngine2D.src;
using GameEngine2D.src.Util;
using GameEngine2D.src.Layer;
using GameEngine2D.src.Entities.Animation;
using GameEngine2D.Engine.src.Entities.Animations;
using GameEngine2D.Engine.src.Entities.Interfaces;
using GameEngine2D.Engine.src.Physics.Raycast;
using GameEngine2D.Engine.src.Layer;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Engine.src.Entities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameEngine2D.Entities
{
    public class Entity : GameObject, Interfaces.IDrawable, IUpdatable, ICollider, IRayBlocker
    {

        private readonly string DESTROY_AMINATION = "Destroy";

        protected Vector2 StartPosition;
        private Vector2 position;

        public bool Visible = true;

        protected Vector2 DrawOffset { get; set; } = Vector2.Zero;

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
        protected SpriteBatch SpriteBatch { get; set; }
        private HashSet<Entity> children;
        private HashSet<IUpdatable> updatables;
        private HashSet<Interfaces.IDrawable> drawables;

        private Entity parent;

        public bool HasCollision { get; set; }

        protected AbstractLayer Layer { get; set; }
        protected List<(Vector2 start, Vector2 end)> RayBlockerLines;

        private bool blocksRay = false;

        public SoundEffect DestroySound;

        protected List<FaceDirection> SinglePointCollisionChecks = new List<FaceDirection>();
        public bool BlocksRay {
            get => blocksRay;
            set
            {
                if (!value)
                {
                    Scene.Instance.RayBlockersLayer.Remove(this);
                }
                else
                {
                    Scene.Instance.RayBlockersLayer.AddObject(this);
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

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        private Texture2D pivot;

        private Dictionary<Entity, bool> collidesWith = new Dictionary<Entity, bool>();

        public Entity(AbstractLayer layer, Entity parent, Vector2 startPosition, SpriteFont font = null)
        {
            this.Layer = layer;
            SpriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
            GridCoordinates = CalculateGridCoord(startPosition);
            children = new HashSet<Entity>();
            updatables = new HashSet<IUpdatable>();
            drawables = new HashSet<Interfaces.IDrawable>();
            if (parent != null) {
                this.parent = parent;
                this.parent.AddChild(this);
                this.StartPosition = this.Position = startPosition;
            } else
            {
                RootContainer.Instance.AddChild(this);
                this.StartPosition = this.Position = startPosition;
            }
            
            HasCollision = true;
            //this.startPosition = this.currentPosition = startPosition;

#if GRAPHICS_DEBUG
            this.font = font;
            pivot = SpriteUtil.CreateCircle(GraphicsDeviceManager, 10, Color.Red);
#endif
            layer.AddObject(this);
        }

        protected virtual void SetRayBlockers()
        {
            /*rayBlockerLines.Add((Vector2.Zero, new Vector2(0, Config.GRID)));
            rayBlockerLines.Add((Vector2.Zero, new Vector2(Config.GRID, 0)));
            rayBlockerLines.Add((new Vector2(Config.GRID, 0), new Vector2(0, Config.GRID)));
            rayBlockerLines.Add((new Vector2(0, Config.GRID), new Vector2(Config.GRID, Config.GRID)));*/

            RayBlockerLines.Add((Position, new Vector2(Position.X, Position.Y + Config.GRID))); //0, 1
            RayBlockerLines.Add((Position, new Vector2(Position.X + Config.GRID, Position.Y))); //1, 0
            RayBlockerLines.Add((new Vector2(Position.X + Config.GRID, Position.Y), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID))); //1
            RayBlockerLines.Add((new Vector2(Position.X, Position.Y + Config.GRID), new Vector2(Position.X + Config.GRID, Position.Y + Config.GRID)));
        }

        public virtual void PreDraw(GameTime gameTime)
        {
            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PreDraw(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            Vector2 position;
            if (parent != null)
            {
                position = StartPosition + parent.GetPositionWithParent();
            } else
            {
                
                position = Position + Layer.GetPosition();
            }

            if (Sprite != null)
            {
                DrawSprite(position);
            }
            else
            {
                if (Visible)
                {
                    Animations.Draw(gameTime);
                }
            }
#if GRAPHICS_DEBUG
            if (Visible)
            {
                SpriteBatch.Begin();
                if (font != null)
                {
                    if (parent != null)
                    {
                        SpriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, StartPosition + parent.GetPositionWithParent(), Color.White);
                    }
                    else
                    {
                        SpriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, Position + Layer.GetPosition(), Color.White);
                    }

                }
                SpriteBatch.Draw(pivot, position, Color.White);
                SpriteBatch.End();
            }
#endif

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.Draw(gameTime);
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
            return Scene.Instance.GetColliderAt(GridCoordinates);
        }

        public Entity GetLeftCollider()
        {
            return Scene.Instance.GetColliderAt(GridUtil.GetLeftGrid(GridCoordinates));
        }

        public Entity GetRightCollider()
        {
            return Scene.Instance.GetColliderAt(GridUtil.GetRightGrid(GridCoordinates));
        }

        public Entity GetTopCollider()
        {
            return Scene.Instance.GetColliderAt(GridUtil.GetUpperGrid(GridCoordinates));
        }

        public Entity GetBottomCollider()
        {
            return Scene.Instance.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates));
        }


        protected virtual void DrawSprite(Vector2 position)
        {
            if (Sprite != null && Visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Sprite, position + DrawOffset, Color.White);
                SpriteBatch.End();
            }
        }
        public bool IsIdle()
        {
            return MathUtil.SmallerEqualAbs(Direction, new Vector2(0.5f, 0.5f));
        }

        public virtual void PostDraw(GameTime gameTime)
        {

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PostDraw(gameTime);
            }
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
            CheckCollisions();

            foreach (IUpdatable child in updatables)
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

            foreach (IUpdatable child in updatables)
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

            if (Scene.Instance.HasColliderAt(GridCoordinates))
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
                updatables.Add((IUpdatable)gameObject);
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
                updatables.Remove((IUpdatable)gameObject);
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
            RootContainer.Instance.RemoveChild(this);
            if (Sprite != null)
            {
                Sprite.Dispose();
            }
            if (DestroySound != null)
            {
                //DestroySound.Dispose();
            }
            Layer.Remove(this);
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
            if (destroyAnimation.StartedAction == null)
            {
                destroyAnimation.StartedAction = () => RemoveCollisions();
                //destroyAnimation.StoppedAction = () => { };
            }
            if (destroyAnimation.StoppedAction == null)
            {
                destroyAnimation.StoppedAction = () => Cleanup();
                //destroyAnimation.StoppedAction = () => { };
            }
            destroyAnimation.Looping = false;
            Animations.RegisterAnimation(DESTROY_AMINATION, destroyAnimation, () => false);
        }

        public void SetSprite(Texture2D texture)
        {
            this.Sprite = texture;
        }

        public Vector2 GetPositionWithParent()
        {
            if (parent != null)
            {
                return Position + parent.GetPositionWithParent();
            }
            else
            {
                return Position + Layer.GetPosition();
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
                RootContainer.Instance.RemoveChild(this);
                RootContainer.Instance.AddChild(newParent);
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
    }
}
