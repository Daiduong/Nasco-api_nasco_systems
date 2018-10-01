using NascoWebAPI.Helper.Common;
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
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }
        public static object GetValue(this object obj, string propertyName)
        {
            if (!obj.GetType().HasProperty(propertyName))
            {
                return 0;
            }
            return obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(obj); ;
        }
        public static int GetStatusLadingTemp(int statusOld)
        {
            if ((int)StatusLading.ChoLayHang == statusOld)
            {
                return (int)StatusLadingTemp.WaitingPickUp;
            }
            if ((int)StatusLading.DangLayHang == statusOld)
            {
                return (int)StatusLadingTemp.PickingUp;
            }
            if ((int)StatusLading.DaLayHang == statusOld)
            {
                return (int)StatusLadingTemp.PickedUp;
            }
            if ((int)StatusLading.LayHangKhongTC == statusOld)
            {
                return (int)StatusLadingTemp.PickedFail;
            }
            if ((int)StatusLading.NVKhongNhan == statusOld)
            {
                return (int)StatusLadingTemp.RefusePickUp;
            }
            if ((int)StatusLading.NhapKho == statusOld)
            {
                return (int)StatusLadingTemp.InStock;
            }
            if ((int)StatusLading.KHTaoBill == statusOld)
            {
                return (int)StatusLadingTemp.Created;
            }
            return (int)StatusLadingTemp.Cancel;
        }
        public static string GetCodeWithMinLegth(int id, ushort length)
        {
            var divide = id / (int)(Math.Pow(10, length));
            length += (ushort)Math.Log10(divide);
            return id.ToString("D" + length);
        }
    }
}
