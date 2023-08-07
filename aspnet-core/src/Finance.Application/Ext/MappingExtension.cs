using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    public static class ObjectReflection
    {
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static T AutoMapping<T>(this object s) where T : class, new()
        {
            var t = new T();
            // get source PropertyInfos
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            // get target type
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);

                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
            return t;
        }

        public static void AutoMapping<S, T>(S s, T t)
        {
            // get source PropertyInfos
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            // get target type
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);

                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
        }
    }
}
