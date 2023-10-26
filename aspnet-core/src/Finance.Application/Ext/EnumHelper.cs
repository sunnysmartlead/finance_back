using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    /// <summary>
    /// 枚举项属性
    /// </summary>
    public class EnumItem
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// 枚举操作公共类
    /// </summary>
    public class EnumHelper
    {
        public static List<EnumItem> GetEnumItems<T>()
        {
            var result = new List<EnumItem>();

            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                return result;
            }

            string[] fieldstrs = Enum.GetNames(enumType);

            foreach (var item in fieldstrs)
            {
                string description = string.Empty;
                var field = enumType.GetField(item);
                object[] arr = field.GetCustomAttributes(typeof(DescriptionAttribute), true); //获取属性字段数组
                if (arr != null && arr.Length > 0)
                {
                    description = ((DescriptionAttribute)arr[0]).Description;   //属性描述
                }
                else
                {
                    description = item;  //描述不存在取字段名称
                }
                result.Add(new EnumItem
                {
                    Code = item,
                    Value = description,

                });
            }

            return result;
        }

 
    }
    /// <summary>
    /// 枚举操作公共类
    /// </summary>
    public class EnumHelperStatus
    {
        /// <summary>
        /// 根据枚举类型得到其所有的 值 与 枚举定义Description属性 的集合
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;


        }
    }
  
}
