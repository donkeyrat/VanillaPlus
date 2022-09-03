using System.Reflection;
using UnityEngine;

namespace VanillaPlus
{
    public static class VPExtensions
    {
        public static T DeepCopyOf<T>(this T self, T from) where T : class
        {
            FieldInfo[] fields = typeof(T).GetFields((BindingFlags)(-1));
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo in array)
            {
                try
                {
                    fieldInfo.SetValue(self, fieldInfo.GetValue(from));
                }
                catch
                {
                    Debug.Log("you suck and you field");
                }
            }
            PropertyInfo[] properties = typeof(T).GetProperties((BindingFlags)(-1));
            PropertyInfo[] array3 = properties;
            foreach (PropertyInfo propertyInfo in array3)
            {
                bool flag = propertyInfo.CanWrite && propertyInfo.CanRead;
                if (flag)
                {
                    try
                    {
                        propertyInfo.SetValue(self, propertyInfo.GetValue(from));
                    }
                    catch
                    {
                        Debug.Log("you suck and you property");
                    }
                }
            }
            return self;
        }
    }
}