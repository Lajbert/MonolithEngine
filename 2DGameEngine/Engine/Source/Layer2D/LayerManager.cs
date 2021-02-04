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
        private List<Layer> parallaxLayers = new List<Layer>();

        List<List<Layer>> allLayers = new List<List<Layer>>();

        public Layer EntityLayer;

        private List<Layer> foregroundLayers = new List<Layer>();

        private List<Layer> backgroundLayers = new List<Layer>();

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
            if (EntityLayer != null)
            {
                throw new Exception("Root already initialized!");
            }
            EntityLayer = new Layer(Camera, 10);

            allLayers.Add(parallaxLayers);
            allLayers.Add(backgroundLayers);
            allLayers.Add(
                new List<Layer>()
                    {
                        EntityLayer,
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
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.Destroy();
                }
            }
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.DrawAll(gameTime);
                }
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.UpdateAll(gameTime);
                }
            }
        }

        public Layer CreateForegroundLayer(int priority = 0)
        {
            Layer l = new Layer(Camera, priority, false);
            AddLayer(foregroundLayers, l);
            return l;
        }

        public Layer CreateBackgroundLayer(int priority = 0)
        {
            Layer l = new Layer(Camera, priority, false);
            AddLayer(backgroundLayers, l);
            return l;
        }

        public Layer CreateParallaxLayer(int priority = 0, float scrollSpeedMultiplier = 1, bool lockY = false)
        {
            Layer l = new Layer(Camera, priority, false, scrollSpeedMultiplier, lockY);
            AddLayer(parallaxLayers, l);
            return l;
        }

        private void AddLayer(List<Layer> layer, Layer newLayer)
        {
            layer.Add(newLayer);
            layer.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }
}
