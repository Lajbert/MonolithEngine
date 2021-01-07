using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.Source.Layer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source
{
    public class Scene
    {

        public SimpleLayer BackgroundLayer { get; } = new SimpleLayer();
        public List<SimpleLayer> ScrollableBackgroundLayers { get; } = new List<SimpleLayer>();
        public ColliderLayer ColliderLayer { get; } = new ColliderLayer();
        public SimpleLayer EntityLayer { get; } = new SimpleLayer();
        public SimpleLayer RayBlockersLayer { get; } = new SimpleLayer();

        private Scene() {

        }
        static Scene() { }

        private static readonly Scene instance = new Scene();

        public bool HasColliderAt(Vector2 location)
        {
            return ColliderLayer.HasObjectAt(location) && ColliderLayer.GetObjectAt(location).HasCollision;
        }

        public Entity GetColliderAt(Vector2 location)
        {
            return ColliderLayer.GetObjectAt(location);
        }

        public static Scene Instance
        {
            get
            {
                return instance;
            }
        }

        public SimpleLayer AddScrollableLayer(float scrollSpeedMultiplier = 1, bool lockY = false)
        {
            ScrollableBackgroundLayers.Add(new SimpleLayer(scrollSpeedMultiplier, lockY));
            return ScrollableBackgroundLayers[ScrollableBackgroundLayers.Count - 1];
        }
    }
}
