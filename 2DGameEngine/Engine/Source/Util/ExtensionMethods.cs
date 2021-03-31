using System;
using System.Collections.Generic;
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
    }
}
