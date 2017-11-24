using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NascoWebAPI.Helper
{
    public static class Extensions
    {
        public static string CommaSeparate<T, U>(this IEnumerable<T> source, Func<T, U> func)
        {
            return string.Join(",", source.Select(s => func(s).ToString()).ToArray());
        }
        public static string CommaSeparatePadded<T, U>(this IEnumerable<T> source, Func<T, U> func)
        {
            return string.Join(", ", source.Select(s => func(s).ToString()).ToArray());
        }

        public static bool IsNumeric(this string s)
        {
            double result;
            return double.TryParse(s, out result);
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static string Ellipsify(this string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s)) return "";

            if (s.Length <= maxLength) return s;

            return s.Substring(0, maxLength) + "...";
        }
      
        public static bool CopyFromJOject<T>(this T obj, JObject jsonData) where T : class
        {
            foreach (var jProp in jsonData)
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.Name.ToUpper() == jProp.Key.ToUpper())
                    {
                        if (jProp.Value != null)
                        {
                            try
                            {
                                prop.SetValue(obj, jProp.Value.ToObject(prop.PropertyType), null);
                                break;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Invalid", ex);
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}