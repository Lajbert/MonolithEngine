using GameEngine2D.Engine.src.Layer;
using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Layer
{
    class ColliderLayer : AbstractLayer
    {
        private Dictionary<Vector2, Entity> objects;
        private float scrollSpeedModifier;
        private bool lockY;

        public ColliderLayer(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.objects = new Dictionary<Vector2, Entity>();
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.lockY = lockY;
        }

        public override void AddObject(Entity gameObject)
        {
            objects.Add(gameObject.GetGridCoord(), gameObject);
        }

        public void RemoveObject(Vector2 position)
        {
            Entity e = objects[position];
            objects.Remove(position);
            foreach (Entity child in e.GetAllChildren()) {
                RemoveObject(e.GetPosition());
            }
        }

        public bool HasObjectAt(Vector2 position)
        {
            return objects.ContainsKey(position);
        }

        public override Vector2 GetPosition()
        {
            if (lockY)
            {
                return new Vector2(RootContainer.Instance.GetRootPosition().X * scrollSpeedModifier, RootContainer.Instance.GetRootPosition().Y);
            }
            return RootContainer.Instance.GetRootPosition() * scrollSpeedModifier;
        }

        public override List<Entity> GetAll()
        {
            return new List<Entity>(objects.Values);
        }

    }
}
