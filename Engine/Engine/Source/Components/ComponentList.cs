using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    public class ComponentList
    {
        private Dictionary<Type, List<IComponent>> components = new Dictionary<Type, List<IComponent>>();

        public T GetComponent<T>() where T : IComponent
        {
#if DEBUG
            if (components.ContainsKey(typeof(T)) && !components[typeof(T)][0].UniquePerEntity)
            {
                Logger.Warn("Using 'GetComponent()' on a non-unique component!");
            }
#endif
            if (!components.ContainsKey(typeof(T)))
            {
                return default(T);
            }
            return (T) components[typeof(T)][0];
        }

        public List<T> GetComponents<T>() where T : IComponent
        {
#if DEBUG
            if (components.ContainsKey(typeof(T)) && components[typeof(T)][0].UniquePerEntity)
            {
                Logger.Warn("Using 'GetComponents()' on a unique component!");
            }
#endif
            if (!components.ContainsKey(typeof(T)))
            {
                return new List<T>();
            }
            List<T> result = new List<T>(components[typeof(T)].Count);
            foreach (IComponent component in components[typeof(T)])
            {
                result.Add((T)component);
            }
            return result;
        }

        public void AddComponent<T>(T newComponent) where T : IComponent
        {
            if (newComponent.UniquePerEntity && (components.ContainsKey(newComponent.GetComponentType()) && components[newComponent.GetComponentType()].Count > 0)) {
                throw new Exception("Can't add more than on of the following component type: " + typeof(T).Name);
            }
            if (!components.ContainsKey(typeof(T)))
            {
                components[newComponent.GetComponentType()] = new List<IComponent>();
            }
            components[newComponent.GetComponentType()].Add(newComponent);
        }

        public void RemoveComponent<T>(T component) where T : IComponent
        {
            components[typeof(T)].Remove(component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            components[typeof(T)].Clear();
        }

        public void Clear<T>() where T : IComponent
        {
            components.Remove(typeof(T));
        }

        public void ClearAll()
        {
            components.Clear();
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (List<IComponent> list in components.Values)
            {
                if (list.Count == 0)
                {
                    continue;
                }
                foreach (IComponent component in list)
                {
                    if (component is IDrawableComponent)
                    {
                        (component as IDrawableComponent).Draw(spriteBatch);
                    }
                }
            }
        }

        public void UpdateAll()
        {
            foreach (List<IComponent> list in components.Values)
            {
                if (list.Count == 0)
                {
                    continue;
                }
                foreach (IComponent component in list)
                {
                    if (component is IUpdatableComponent)
                    {
                        (component as IUpdatableComponent).Update();
                    }
                }
            }
        }
    }
}
