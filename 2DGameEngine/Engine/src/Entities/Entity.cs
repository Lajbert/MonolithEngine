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

namespace GameEngine2D.Entities
{
    public class Entity : GameObject, Interfaces.IDrawable, IUpdatable, ICollider, IRayBlocker
    {

        protected Vector2 StartPosition;
        private Vector2 position;

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
        public bool BlocksRay {
            get => blocksRay;
            set
            {
                if (!value)
                {
                    Scene.Instance.RayBlockersLayer.RemoveObject(this);
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

            if (Animations != null)
            {
                Animations.Draw(gameTime);
            }
            else
            {
                DrawSprite(position);
            }
#if GRAPHICS_DEBUG
            spriteBatch.Begin();
            if (font != null)
            {
                if (GetParent() != null)
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, startPosition + GetParent().GetPositionWithParent(), Color.White);
                } else
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, currentPosition + layer.GetPosition(), Color.White);
                }
                
            }
            spriteBatch.End();
#endif

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.Draw(gameTime);
            }
        }


        protected virtual void DrawSprite(Vector2 position)
        {
            if (Sprite != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Sprite, position, Color.White);
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
            parent.RemoveChild(this);
            if (!children.Any())
            {
                foreach (Entity o in children) {
                    if (o != null) {
                        o.Destroy();
                    }
                }
            }
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
