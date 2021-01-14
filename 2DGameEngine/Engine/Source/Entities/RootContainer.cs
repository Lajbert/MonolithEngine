using System;
using System.Collections.Generic;
using System.Text;
using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.Layer;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities
{
    public class RootContainer : GameObject
    {
        private List<Layer> layers = new List<Layer>();

        public Layer EntityLayer;

        public Layer RayBlockersLayer;

        public Layer ForegroundEnvironment;

        public Layer BackgroundEnviornment;

        private Vector2 position = Vector2.Zero;

        public Camera Camera;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public float X {
            set { position.X = value; }
            get { return position.X; } 
        }

        public float Y
        {
            set { position.Y = value; }
            get { return position.Y; }
        }

        private static readonly RootContainer instance = new RootContainer();

        private RootContainer()
        {

        }
        static RootContainer()
        {
        }

        public void InitLayers()
        {
            if (EntityLayer != null || RayBlockersLayer != null)
            {
                throw new Exception("Root already initialized!");
            }
            EntityLayer = new Layer(Camera, 10);
            RayBlockersLayer = new Layer(Camera);
        }

        public static RootContainer Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Destroy()
        {
            foreach (Layer layer in layers)
            {
                layer.Destroy();
            }
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (Layer layer in layers)
            {
                layer.DrawAll(gameTime);
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (Layer layer in layers)
            {
                layer.UpdateAll(gameTime);
            }
        }

        public void AddLayer(Layer layer)
        {
            layers.Add(layer);
            layers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public Layer CreateParallaxLayer(int priority = 0, float scrollSpeedMultiplier = 1, bool lockY = false)
        {
            Layer l = new Layer(Camera, priority, false, scrollSpeedMultiplier, lockY);
            AddLayer(l);
            return l;
        }
    }
}
