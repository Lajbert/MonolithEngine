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
    class Scene
    {

        private SimpleLayer background;
        private List<SimpleLayer> scrollableBackgrounds;
        private ColliderLayer colliders;
        private SimpleLayer entities;
        private SimpleLayer rayBlockers;

        private Scene() {
            background = new SimpleLayer();
            colliders = new ColliderLayer();
            entities = new SimpleLayer();
            rayBlockers = new SimpleLayer();
            scrollableBackgrounds = new List<SimpleLayer>();
        }
        static Scene() { }

        private static readonly Scene instance = new Scene();

        public bool HasColliderAt(Vector2 location)
        {
            return colliders.HasObjectAt(location);
        }

        public static Scene Instance
        {
            get
            {
                return instance;
            }
        }

        public ColliderLayer GetColliderLayer()
        {
            return colliders;
        }

        public SimpleLayer GetEntityLayer()
        {
            return entities;
        }

        public SimpleLayer GetRayBlockersLayer()
        {
            return rayBlockers;
        }

        public SimpleLayer GetBackgroundLayer()
        {
            return this.background;
        }

        public SimpleLayer GetScrollableLayer(int index)
        {
            return scrollableBackgrounds[index];
        }

        public void AddScrollableLayer(float scrollSpeedMultiplier, bool lockY = false)
        {
            scrollableBackgrounds.Add(new SimpleLayer(scrollSpeedMultiplier, lockY));
        }
    }
}
