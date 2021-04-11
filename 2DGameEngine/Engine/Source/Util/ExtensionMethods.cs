using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonolithEngine.Engine.Source.Util
{
    public static class ExtensionMethods
    {
        public static void AddIfMissing<T>(this HashSet<T> set, T newElement)
        {
            if (!set.Contains(newElement))
            {
                set.Add(newElement);
            }
        }

        public static void RemoveIfExists<T>(this HashSet<T> set, T newElement)
        {
            if (set.Contains(newElement))
            {
                set.Remove(newElement);
            }
        }

        public static void RemoveIfExists<T, V>(this Dictionary<T, V> map, T toRemove)
        {
            if (map.ContainsKey(toRemove))
            {
                map.Remove(toRemove);
            }
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
