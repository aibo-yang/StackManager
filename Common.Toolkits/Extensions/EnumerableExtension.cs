using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Toolkits.Extensions
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this T[] source)
        {
            if (source != null)
            {
                return source.Length == 0;
            }

            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return false;
            }
            return true;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (!source.IsNullOrEmpty())
            {
                foreach (T element in source)
                {
                    action(element);
                }
            }
        }

        public static void ForEach<T>(this T[] source, Action<T> action)
        {
            if (!source.IsNullOrEmpty())
            {
                for (int i = 0; i < source.Length; i++)
                {
                    action(source[i]);
                }
            }
        }

        public static IEnumerable<T> Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (!source.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(source));    
            }
            
            foreach (var e in source)
            {
                action(e);
                yield return e;
            }
        }

        public static IEnumerable<T> Done<T>(this IEnumerable<T> source)
        {
            if (!source.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var e in source)
            {
                ;
            }
            return source;
        }

        public static bool ChildrenIsEqual<T>(this IEnumerable<T> sources, IEnumerable<T> thats, Func<T, T, bool> predicate) where T : class
        {
            if (sources == null || thats == null || sources.Count() != thats.Count())
            {
                return false;
            }

            foreach (var source in sources)
            {
                if (!thats.Any(x=> predicate(source, x)))
                {
                    return false;
                }
            }

            foreach (var that in thats)
            {
                if (!sources.Any(x => predicate(x, that)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
