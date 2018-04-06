using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public static class Helper
    {
        public static string GetEnumDescription(Enum value)
        {
            System.Reflection.FieldInfo fi = System.Reflection.TypeExtensions.GetField(value.GetType(), value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static IEnumerable<T> Recurse<T>
            (
                this T root,
                Func<T, IEnumerable<T>> findChildren
            )
            where T : class
        {
            yield return root;

            foreach (var child in
                findChildren(root)
                    .SelectMany(node => Recurse(node, findChildren))
                    .TakeWhile(child => child != null))
            {
                yield return child;
            }
        }

    }
}
