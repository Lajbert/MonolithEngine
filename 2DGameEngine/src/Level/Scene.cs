using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using _2DGameEngine.Global;
using _2DGameEngine.src.Layer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src
{
    class Scene
    {

        private GraphicsLayer background;
        private List<GraphicsLayer> scrollableBackgrounds;
        private GraphicsLayer colliders;
        private GraphicsLayer entities;

        private Scene() {
            background = new GraphicsLayer();
            colliders = new GraphicsLayer();
            entities = new GraphicsLayer();
            scrollableBackgrounds = new List<GraphicsLayer>();
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

        public GraphicsLayer GetColliderLayer()
        {
            return colliders;
        }

        public GraphicsLayer GetEntityLayer()
        {
            return entities;
        }

        public GraphicsLayer GetBackgroundLayer()
        {
            return this.background;
        }

        public GraphicsLayer GetScrollableLayer(int index)
        {
            return scrollableBackgrounds[index];
        }

        public void AddScrollableLayer(float scrollSpeedMultiplier, bool lockY = false)
        {
            scrollableBackgrounds.Add(new GraphicsLayer(scrollSpeedMultiplier, lockY));
        }
    }
}
