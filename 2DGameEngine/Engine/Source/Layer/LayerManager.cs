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
    public class LayerManager : GameObject
    {
        private List<Layer2D> parallaxLayers = new List<Layer2D>();

        List<List<Layer2D>> allLayers = new List<List<Layer2D>>();

        public Layer2D EntityLayer;

        public Layer2D RayBlockersLayer;

        private List<Layer2D> foregroundLayers = new List<Layer2D>();

        private List<Layer2D> backgroundLayers = new List<Layer2D>();

        public Camera Camera;

        private static readonly LayerManager instance = new LayerManager();

        private LayerManager()
        {

        }
        static LayerManager()
        {
        }

        public void InitLayers()
        {
            if (EntityLayer != null || RayBlockersLayer != null)
            {
                throw new Exception("Root already initialized!");
            }
            EntityLayer = new Layer2D(Camera, 10);
            RayBlockersLayer = new Layer2D(Camera);

            allLayers.Add(parallaxLayers);
            allLayers.Add(backgroundLayers);
            allLayers.Add(
                new List<Layer2D>()
                    {
                        EntityLayer,
                        RayBlockersLayer
                    }
                );
            allLayers.Add(foregroundLayers);
        }

        public static LayerManager Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Destroy()
        {
            foreach (List<Layer2D> layers in allLayers)
            {
                foreach (Layer2D l in layers)
                {
                    l.Destroy();
                }
            }
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (List<Layer2D> layers in allLayers)
            {
                foreach (Layer2D l in layers)
                {
                    l.DrawAll(gameTime);
                }
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (List<Layer2D> layers in allLayers)
            {
                foreach (Layer2D l in layers)
                {
                    l.UpdateAll(gameTime);
                }
            }
        }

        public Layer2D CreateForegroundLayer(int priority = 0)
        {
            Layer2D l = new Layer2D(Camera, priority, false);
            AddLayer(foregroundLayers, l);
            return l;
        }

        public Layer2D CreateBackgroundLayer(int priority = 0)
        {
            Layer2D l = new Layer2D(Camera, priority, false);
            AddLayer(backgroundLayers, l);
            return l;
        }

        public Layer2D CreateParallaxLayer(int priority = 0, float scrollSpeedMultiplier = 1, bool lockY = false)
        {
            Layer2D l = new Layer2D(Camera, priority, false, scrollSpeedMultiplier, lockY);
            AddLayer(parallaxLayers, l);
            return l;
        }

        private void AddLayer(List<Layer2D> layer, Layer2D newLayer)
        {
            layer.Add(newLayer);
            layer.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }
}
