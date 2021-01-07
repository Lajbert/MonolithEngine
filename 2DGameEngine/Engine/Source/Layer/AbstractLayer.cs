using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Layer
{
    public abstract class AbstractLayer
    {
        public float LayerDepth { get; set; } = 0;
        public abstract void AddObject(Entity gameObject);
        public abstract Vector2 GetPosition();

        public abstract IEnumerable<Entity> GetAll();

        public abstract void Remove(Entity gameObject);
    }
}
