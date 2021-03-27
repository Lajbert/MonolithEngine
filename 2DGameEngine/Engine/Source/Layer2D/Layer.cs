using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Entities.Interfaces;
using MonolithEngine.Global;
using MonolithEngine.Source.Camera2D;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Source.GridCollision
{
    public class Layer
    {
        private List<Entity> activeObjects = new List<Entity>();
        private List<Entity> visibleObjects = new List<Entity>();

        private List<Entity> changedObjects = new List<Entity>();

        private float scrollSpeedModifier;
        private bool lockY;
        private bool ySorting = false;
        private SpriteBatch spriteBatch;

        public bool Visible = true;
        public bool Active = true;

        public static GraphicsDeviceManager GraphicsDeviceManager;

        public int Priority = 0;

        public float Depth { get; set; } = 0;

        public Camera Camera;

        internal Layer(Camera camera, int priority = 0, bool ySorting = false, float scrollSpeedModifier = 1f, bool lockY = true)
        {
            if (camera == null)
            {
                throw new Exception("Camera not provided for layer!");
            }
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.Camera = camera;
            Priority = priority;
            this.lockY = lockY;
            this.ySorting = ySorting;
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
        }

        public void OnObjectChanged(Entity gameObject)
        {
            changedObjects.Add(gameObject);
        }

        public IEnumerable<Entity> GetAll()
        {
            return visibleObjects;
        }

        public void SortByPriority()
        {
            if (visibleObjects.Count == 0)
            {
                return;
            }
            visibleObjects.Sort((a, b) => a.DrawPriority.CompareTo(b.DrawPriority));
        }

        public void DrawAll(GameTime gameTime)
        {

            /*if (!Visible)
            {
                return;
            }*/

            if (Visible)
            {
                if (ySorting)
                {
                    visibleObjects.Sort((a, b) => {
                        int res = a.DrawPriority.CompareTo(b.DrawPriority);
                        if (res != 0) return res;
                        return a.Transform.Y.CompareTo(b.Transform.Y);
                    });
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.GetTransformMatrix(scrollSpeedModifier, lockY));

                foreach (Entity entity in visibleObjects)
                {
                    entity.Draw(spriteBatch, gameTime);
                }
                spriteBatch.End();
            }

            HandleChangedObjects();
        }

        public void UpdateAll()
        {

            /*if (!Active)
            {
                return;
            }*/

            // in case of skipped frame, we should just recalculate everything

            if (Globals.ElapsedTime > 1000)
            {
                return;
            }
            if (Active)
            {
                foreach (Entity entity in activeObjects)
                {
                    entity.PreUpdate();
                    entity.Update();
                    entity.PostUpdate();
                }
            }

            HandleChangedObjects();
        }

        public void FixedUpdateAll()
        {

            /*if (!Active)
            {
                return;
            }*/

            // in case of skipped frame, we should just recalculate everything
            if (Globals.ElapsedTime > 1000)
            {
                return;
            }
            if (Active)
            {
                foreach (Entity entity in activeObjects)
                {
                    entity.FixedUpdate();
                }
            }

            HandleChangedObjects();
        }

        private void HandleChangedObjects()
        {
            if (changedObjects.Count > 0)
            {
                foreach (Entity e in changedObjects)
                {
                    if (e.Visible)
                    {
                        if (e.Parent == null && !visibleObjects.Contains(e))
                        {
                            visibleObjects.Add(e);
                        }
                        else if (e.Parent != null && visibleObjects.Contains(e))
                        {
                            visibleObjects.Remove(e);
                        }
                    }
                    else
                    {
                        visibleObjects.Remove(e);
                    }
                    if (e.Active)
                    {
                        if (e.Parent == null && !activeObjects.Contains(e))
                        {
                            activeObjects.Add(e);
                        }
                        else if (e.Parent != null && activeObjects.Contains(e))
                        {
                            activeObjects.Remove(e);
                        }
                    }
                    else
                    {
                        activeObjects.Remove(e);
                    }
                }
                changedObjects.Clear();
                SortByPriority();
            }
        }

        public void Destroy()
        {
            foreach (Entity entity in activeObjects)
            {
                entity.Destroy();
            }
        }

    }
}
