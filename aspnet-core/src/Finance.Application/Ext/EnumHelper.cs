using NPOI.SS.Formula.Functions;
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

        /// <summary>
        /// 根据枚举获取描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCodeFromEnum(string code)
        {
            if (null == code || code.Equals(""))
            {
                return "";
            }
            var res = EnumHelper.GetEnumItems<Status>();
            for (int i = 0; i < res.Count; i++)
            {
                if (code.Equals(res[i].Code))
                {

                    return res[i].Value;
                }
            }

            return "";
        }

        /// <summary>
        /// 根据获取描述枚举
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GettDescriptionFromEnum(string value)
        {
            if (null == value || value.Equals(""))
            {
                return "";
            }
            var res = EnumHelper.GetEnumItems<Status>();
            for (int i = 0; i < res.Count; i++)
            {
                if (value.Equals(res[i].Value))
                {

                    return res[i].Code;
                }
            }

            return "";
        }


    }



  
}
