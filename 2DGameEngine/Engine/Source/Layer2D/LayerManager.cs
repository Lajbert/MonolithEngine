using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Entities;
using MonolithEngine.Entities.Interfaces;
using MonolithEngine.Global;
using MonolithEngine.Source.GridCollision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine.Engine.Source.Scene;

namespace MonolithEngine.Entities
{
    public class LayerManager
    {
        private List<Layer> parallaxLayers = new List<Layer>();

        List<List<Layer>> allLayers = new List<List<Layer>>();

        public Layer EntityLayer;

        private List<Layer> foregroundLayers = new List<Layer>();

        private List<Layer> backgroundLayers = new List<Layer>();

        private AbstractScene scene;

        public LayerManager(AbstractScene scene)
        {
            this.scene = scene;
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
            EntityLayer = new Layer(scene, 10);

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

        public void Destroy()
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.Destroy();
                }
                layers.Clear();
            }
            allLayers.Clear();
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.DrawAll(spriteBatch);
                }
            }
            //spriteBatch.End();
        }

        public void UpdateAll()
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.UpdateAll();
                }
            }
        }

        public void FixedUpdateAll()
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    l.FixedUpdateAll();
                }
            }
        }

        public Layer CreateForegroundLayer(int priority = 0)
        {
            Layer l = new Layer(scene, priority, false);
            AddLayer(foregroundLayers, l);
            return l;
        }

        public Layer CreateBackgroundLayer(int priority = 0)
        {
            Layer l = new Layer(scene, priority, false);
            AddLayer(backgroundLayers, l);
            return l;
        }

        public Layer CreateParallaxLayer(int priority = 0, float scrollSpeedMultiplier = 1, bool lockY = false)
        {
            Layer l = new Layer(scene, priority, false, scrollSpeedMultiplier, lockY);
            AddLayer(parallaxLayers, l);
            return l;
        }

        private void AddLayer(List<Layer> layer, Layer newLayer)
        {
            layer.Add(newLayer);
            layer.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        private void RemoveLayer(List<Layer> layer, Layer toRemove)
        {
            layer.Remove(toRemove);
            layer.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }
}
