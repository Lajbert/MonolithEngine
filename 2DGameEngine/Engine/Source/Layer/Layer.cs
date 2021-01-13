﻿using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.Source.Camera2D;
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
        private List<Entity> newObjects = new List<Entity>();
        private List<Entity> removedObjects = new List<Entity>();
        private float scrollSpeedModifier;
        private bool lockY;

        private SpriteBatch spriteBatch;

        public static GraphicsDeviceManager GraphicsDeviceManager;

        public int Priority = 0;

        public float Depth { get; set; } = 0;

        private Camera camera;

        public Layer(Camera camera, int priority = 0, float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.camera = camera;
            Priority = priority;
            this.lockY = lockY;
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
            RootContainer.Instance.AddLayer(this);
        }

        public void AddRootObject(Entity gameObject)
        {
            newObjects.Add(gameObject);
        }

        private void RemoveObject(Entity gameObject)
        {
            removedObjects.Add(gameObject);
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
            //Vector2 origin = -Camera.Camera.Position + new Vector2(Config.RES_W / 2, Config.RES_H / 2);
            Vector2 origin = new Vector2(Config.RES_W / 2, Config.RES_H / 2);
            //Vector2 origin = Camera.Camera.target.Position + new Vector2(Camera.Camera.target.SourceRectangle.Width, Camera.Camera.target.SourceRectangle.Height) / 2f;
            Matrix matrix = new Matrix();
            matrix =
            //Matrix.CreateTranslation(-RootContainer.Instance.Position.X, -RootContainer.Instance.Position.Y, 0) *
            //Matrix.CreateTranslation(0, 0, 0) *
            //Matrix.CreateTranslation(new Vector3(-Camera.Camera.Position.X + Config.RES_W / 2, -Camera.Camera.Position.Y + Config.RES_H / 2, 0)) *

            Matrix.CreateTranslation(new Vector3(-Camera2D.Camera.Position + new Vector2(Config.RES_W / 2, Config.RES_H / 2), 0)) *
            Matrix.CreateTranslation(new Vector3(-origin, 0)) *
            Matrix.CreateScale(Config.ZOOM, Config.ZOOM, 1) *
            Matrix.CreateTranslation(new Vector3(origin, 0));
            //Matrix.CreateTranslation(new Vector3(-origin, 0));

            //Matrix.CreateTranslation(new Vector3(Camera.Camera.Position.X + Config.RES_W / 2, Camera.Camera.Position.Y + Config.RES_H / 2, 0));
            //Matrix.CreateTranslation(new Vector3(-Camera.Camera.Position.X + 1920 / 2, -Camera.Camera.Position.Y + 1080 / 2, 0));
            //Matrix.CreateTranslation(new Vector3(RootContainer.Instance.X +  1920 / 2, RootContainer.Instance.Y + 1080 / 2, 0));
            //public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.GetTransformMatrix());
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            //spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null);
            foreach (Entity entity in rootObjects)
            {
                entity.PreDraw(spriteBatch, gameTime);
                entity.Draw(spriteBatch, gameTime);
                entity.PostDraw(spriteBatch, gameTime);
            }
            spriteBatch.End();
            if (newObjects.Count > 0)
            {
                rootObjects.AddRange(newObjects);
                newObjects.Clear();
            }
            if (removedObjects.Count > 0)
            {
                foreach (Entity toRemove in removedObjects)
                {
                    rootObjects.Remove(toRemove);
                }
                removedObjects.Clear();
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
            if (newObjects.Count > 0)
            {
                rootObjects.AddRange(newObjects);
                newObjects.Clear();
            }
            if (removedObjects.Count > 0)
            {
                foreach (Entity toRemove in removedObjects)
                {
                    rootObjects.Remove(toRemove);
                }
                removedObjects.Clear();
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