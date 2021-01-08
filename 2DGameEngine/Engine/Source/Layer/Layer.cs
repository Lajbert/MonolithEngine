﻿using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Layer
{
    public class Layer
    {
        private List<Entity> rootObjects = new List<Entity>();
        private float scrollSpeedModifier;
        private bool lockY;

        private SpriteBatch spriteBatch;

        public static GraphicsDeviceManager GraphicsDeviceManager;

        public int Priority = 0;

        public float Depth { get; set; } = 0;

        public Layer(int priority = 0, float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.scrollSpeedModifier = scrollSpeedModifier;
            Priority = priority;
            this.lockY = lockY;
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
            RootContainer.Instance.AddLayer(this);
        }

        public void AddRootObject(Entity gameObject)
        {
            rootObjects.Add(gameObject);
        }

        private void RemoveObject(Entity position)
        {
            rootObjects.Remove(position);
            foreach (Entity child in position.GetAllChildren()) {
                RemoveObject(child);
            }
        }

        public Vector2 GetPosition()
        {
            if (lockY)
            {
                return new Vector2(RootContainer.Instance.Position.X * scrollSpeedModifier, RootContainer.Instance.Position.Y);
            }
            return RootContainer.Instance.Position * scrollSpeedModifier;
        }

        public IEnumerable<Entity> GetAll()
        {
            return rootObjects;
        }

        public void RemoveRoot(Entity gameObject)
        {
            RemoveObject(gameObject);
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (Entity entity in rootObjects)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null);
                entity.PreDraw(spriteBatch, gameTime);
                entity.Draw(spriteBatch, gameTime);
                entity.PostDraw(spriteBatch, gameTime);
                spriteBatch.End();
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (Entity entity in rootObjects)
            {
                entity.PreUpdate(gameTime);
                entity.Update(gameTime);
                entity.PostUpdate(gameTime);
            }
        }

        public void Destroy()
        {
            foreach (Entity entity in rootObjects)
            {
                entity.Destroy();
            }
        }

    }
}
