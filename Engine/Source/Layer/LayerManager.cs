using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// A class contain the layer instances of a game.
    /// Updates and draws layer in the right order to create foreground,
    /// background and parallax effects.
    /// </summary>
    public class LayerManager
    {
        private List<Layer> parallaxLayers = new List<Layer>();

        List<List<Layer>> allLayers = new List<List<Layer>>();

        public Layer EntityLayer;

        internal Layer UILayer;

        private List<Layer> foregroundLayers = new List<Layer>();

        private List<Layer> backgroundLayers = new List<Layer>();

        private AbstractScene scene;

        internal bool Paused = false;

        public LayerManager(AbstractScene scene)
        {
            this.scene = scene;
        }
        static LayerManager()
        {
        }

        public void InitLayers()
        {
            EntityLayer = new Layer(scene, 10);

            UILayer = new Layer(scene, 10);
            UILayer.Pausable = false;

            allLayers.Add(parallaxLayers);
            allLayers.Add(backgroundLayers);
            allLayers.Add(
                new List<Layer>()
                    {
                        EntityLayer,
                    }
                );
            allLayers.Add(foregroundLayers);
            allLayers.Add(
                new List<Layer>()
                    {
                        UILayer,
                    }
                );
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
                    if (!Paused || !l.Pausable)
                    {
                        l.UpdateAll();
                    }
                }
            }
        }

        public void FixedUpdateAll()
        {
            foreach (List<Layer> layers in allLayers)
            {
                foreach (Layer l in layers)
                {
                    if (!Paused || !l.Pausable)
                    {
                        l.FixedUpdateAll();
                    }
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
