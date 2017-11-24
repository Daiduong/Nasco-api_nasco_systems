using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace NascoWebAPI.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            System.Reflection.FieldInfo fi = System.Reflection.TypeExtensions.GetField(value.GetType(), value.ToString());
            if (fi != null)
            {
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
            return string.Empty;
        }
        public static T IntToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }
        public static T StringToEnum<T>(string str)
        {
            T myEnum = (T)Enum.Parse(typeof(T), str);
            // the foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
            if (!Enum.IsDefined(typeof(T), myEnum) && !myEnum.ToString().Contains(","))
                throw new InvalidOperationException($"{str} is not an underlying value of the YourEnum enumeration.");
            return myEnum;
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
