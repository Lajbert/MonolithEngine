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
    class SimpleLayer : AbstractLayer
    {
        private List<Entity> objects;
        private float scrollSpeedModifier;
        private bool lockY;

        public SimpleLayer(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.objects = new List<Entity>();
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.lockY = lockY;
        }

        public override void AddObject(Entity gameObject)
        {
            objects.Add(gameObject);
        }

        public void RemoveObject(Entity position)
        {
            objects.Remove(position);
            foreach (Entity child in position.GetAllChildren()) {
                RemoveObject(child);
            }
        }

        public override Vector2 GetPosition()
        {
            if (lockY)
            {
                return new Vector2(RootContainer.Instance.Position.X * scrollSpeedModifier, RootContainer.Instance.Position.Y);
            }
            return RootContainer.Instance.Position * scrollSpeedModifier;
        }

        public override IEnumerable<Entity> GetAll()
        {
            return objects;
        }

    }
}
