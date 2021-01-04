using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.src.Layer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src
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
            return ColliderLayer.HasObjectAt(location);
        }

        public static Scene Instance
        {
            get
            {
                return instance;
            }
        }

        public void AddScrollableLayer(float scrollSpeedMultiplier, bool lockY = false)
        {
            ScrollableBackgroundLayers.Add(new SimpleLayer(scrollSpeedMultiplier, lockY));
        }
    }
}
