﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Represents a collection of entities in a game instance. Layers can have entities assigned,
    /// each layer is updated separately from the other and drawn using it's own SpriteBatch. 
    /// Layers can be used to display foregroun, background, parallax graphical layers, or to handle
    /// entities that can logically be grouped together.
    /// </summary>
    public class Layer
    {
        private List<Entity> activeObjects = new List<Entity>();
        private List<Entity> visibleObjects = new List<Entity>();

        private List<Entity> changedObjects = new List<Entity>();

        private float scrollSpeedModifier;
        private bool lockY;
        private bool ySorting = false;

        public bool Visible = true;
        public bool Active = true;

        internal bool Pausable = true;

        public static GraphicsDeviceManager GraphicsDeviceManager;

        public int Priority = 0;

        public float Depth { get; set; } = 0;

        public AbstractScene Scene;

        internal Layer(AbstractScene scene, int priority = 0, bool ySorting = false, float scrollSpeedModifier = 1f, bool lockY = true)
        {
            this.scrollSpeedModifier = scrollSpeedModifier;
            Priority = priority;
            this.lockY = lockY;
            this.ySorting = ySorting;
            Scene = scene;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawAll(SpriteBatch spriteBatch)
        {

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
                Viewport vpBackup = GraphicsDeviceManager.GraphicsDevice.Viewport;
                foreach (Camera camera in Scene.Cameras)
                {
                    GraphicsDeviceManager.GraphicsDevice.Viewport = camera.Viewport;
                    Scene.CurrentCamera = camera;
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.GetWorldTransformMatrix(scrollSpeedModifier, lockY));

                    foreach (Entity entity in visibleObjects)
                    {
                        if (entity.Visible)
                        {
                            entity.Draw(spriteBatch);
                        }
                    }
                    spriteBatch.End();
                }
                GraphicsDeviceManager.GraphicsDevice.Viewport = vpBackup;
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
                    if (entity.Active)
                    {
                        entity.PreUpdate();
                        entity.Update();
                        entity.PostUpdate();
                    }
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
                    if (entity.Active)
                    {
                        entity.PreFixedUpdate();
                        entity.FixedUpdate();
                    }
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

            foreach(Entity entity in visibleObjects)
            {
                entity.Destroy();
            }

            foreach (Entity entity in changedObjects)
            {
                entity.Destroy();
            }

            HandleChangedObjects();
        }

    }
}
