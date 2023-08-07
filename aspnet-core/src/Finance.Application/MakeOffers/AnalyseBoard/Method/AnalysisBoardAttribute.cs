using NPOI.HPSF;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Method
{
    public static class AnalysisBoardAttribute
    {
        public static decimal GetDecimal(this decimal @decimal, int count = 2)
        {
            return Math.Round(@decimal, count);
        }
        /// <summary>
        /// 集合中decimal/double类型属性保存两位数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lsit"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> RetainDecimals<T>(this List<T> @lsit,int count=2)
        {
            foreach (var props in @lsit)
            {
                foreach (var item in props.GetType().GetRuntimeProperties())
                {
                    var dtlType = item.PropertyType.Name;
                    if (dtlType == typeof(decimal).Name||dtlType==typeof(double).Name)
                    {                    
                        item.SetValue(props, Math.Round((decimal)item.GetValue(props), count));                      
                    }
                    if (dtlType==typeof(object).Name)
                    {
                        object obj = RetainObject(item.GetValue(props), count);
                        item.SetValue(props, obj);
                    }
                    if (dtlType.Contains("List"))
                    {
                        IEnumerable objects1 = item.GetValue(props) as IEnumerable;
                        foreach (var o in objects1)
                        {
                            foreach (var oitem in o.GetType().GetRuntimeProperties())
                            {
                                var oType = oitem.PropertyType.Name;
                                if (oType == typeof(decimal).Name)
                                {
                                    oitem.SetValue(o, Math.Round((decimal)oitem.GetValue(o), count));
                                }
                                if (oType==typeof(object).Name)
                                {
                                    object obj = RetainObject(oitem.GetValue(o), count);
                                    oitem.SetValue(o, obj);
                                }
                                if (oType.Contains("List"))
                                {
                                    IEnumerable objects = RetainList(oitem.GetValue(o) as IEnumerable, count);
                                    oitem.SetValue(o, objects);
                                }
                            }
                        }
                    }
                }
            }
            return  @lsit;
        }
        /// <summary>
        /// 对象中decimal/double类型属性保存两位数据扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T RetainDecimals<T>(this T @object, int count = 2)
        {
            foreach (var item in @object.GetType().GetRuntimeProperties())
            {
                var dtlType = item.PropertyType.Name;
                if (dtlType == typeof(decimal).Name||dtlType==typeof(double).Name)
                {
                    var prop = item.GetValue(@object);
                    item.SetValue(@object, Math.Round((decimal)item.GetValue(@object), count));
                    var prop2 = item.GetValue(@object);
                }
                if (dtlType==typeof(object).Name)
                {
                    object obj = RetainObject(item.GetValue(@object), count);
                    item.SetValue(@object, obj);
                }
                if (dtlType.Contains("List"))
                {
                    IEnumerable objects1 = item.GetValue(@object) as IEnumerable;
                    foreach (var o in objects1)
                    {
                        foreach (var oitem in o.GetType().GetRuntimeProperties())
                        {
                            var oType = oitem.PropertyType.Name;
                            if (oType == typeof(decimal).Name)
                            {
                                oitem.SetValue(o, Math.Round((decimal)oitem.GetValue(o), count));
                            }
                            if (oType==typeof(object).Name)
                            {
                                object obj = RetainObject(oitem.GetValue(o), count);
                                oitem.SetValue(o, obj);
                            }
                            if (oType.Contains("List"))
                            {
                                IEnumerable objects = RetainList(oitem.GetValue(o) as IEnumerable, count);
                                oitem.SetValue(o, objects);
                            }
                        }
                    }
                }
            }
            return (T)@object;
        }
        public static IEnumerable RetainList(IEnumerable @object, int count = 2)
        {
            foreach (var item in @object)
            {
                foreach (var p in item.GetType().GetRuntimeProperties())
                {
                    var dtlType = p.PropertyType.Name;
                    if (dtlType == typeof(decimal).Name||dtlType==typeof(double).Name)
                    {
                        p.SetValue(item, Math.Round((decimal)p.GetValue(item), count));
                    }
                    if (dtlType==typeof(object).Name)
                    {
                        object obj = RetainObject(p.GetValue(item), count);
                        p.SetValue(item, obj);
                    }
                    if (dtlType.Contains("List"))
                    {
                        var value = p.GetValue(item);
                        IEnumerable en = p.GetValue(item) as IEnumerable;
                        foreach (var pp in en)
                        {
                            foreach (var enp in pp.GetType().GetRuntimeProperties())
                            {
                                var pType = enp.PropertyType.Name;
                                if (dtlType.Contains("List"))
                                {
                                    IEnumerable objects = RetainList(enp.GetValue(pp) as IEnumerable, count);
                                    enp.SetValue(item, objects);
                                }
                            }
                        }

                    }
                }
            }
            return @object;
        }
        public static object RetainObject(object @object, int count = 2)
        {
            foreach (var item in @object.GetType().GetRuntimeProperties())
            {
                var dtlType = item.PropertyType.Name;
                if (dtlType == typeof(decimal).Name || dtlType == typeof(double).Name)
                {
                    var prop = item.GetValue(@object);
                    item.SetValue(@object, Math.Round((decimal)item.GetValue(@object), count));
                    var prop2 = item.GetValue(@object);
                }
                if (dtlType==typeof(object).Name)
                {
                    object obj = RetainObject(item.GetValue(@object), count);
                    item.SetValue(@object, obj);
                }
                if (dtlType.Contains("List"))
                {
                    var value = item.GetValue(@object);
                    IEnumerable en = item.GetValue(@object) as IEnumerable;
                    foreach (var pp in en)
                    {
                        foreach (var p in pp.GetType().GetRuntimeProperties())
                        {
                            var pType = p.PropertyType.Name;
                            if (dtlType.Contains("List"))
                            {
                                IEnumerable objects = RetainList(p.GetValue(pp) as IEnumerable, count);
                                item.SetValue(@object, objects);
                            }
                        }
                    }

                }
            }
            return @object;
        }

        public static string TypeTransition(string type)
        {
            switch (type)
            {
                case "1": return "我司推荐";
                case "2": return "客户指定";
                case "3": return "客户供应";
                default: return "";
            }
        }
    }
}
