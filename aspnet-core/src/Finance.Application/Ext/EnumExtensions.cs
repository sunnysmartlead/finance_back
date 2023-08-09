using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 字符串转枚举扩展
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 将字符串解析为指定枚举类型的值，不区分大小写。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <param name="value">要解析的字符串。</param>
        /// <returns>解析后的枚举值。</returns>
        /// <exception cref="ArgumentException">当方法或构造函数的参数不符合预期的格式、类型或值时，将抛出ArgumentException异常。该异常通常用于检测并处理输入错误。</exception>
        public static T ParseEnum<T>(this string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("将字符串解析为指定枚举类型的值,值不能为空或空白。");
            }

            if (!Enum.TryParse<T>(value, true, out T result))
            {
                throw new ArgumentException($"将字符串解析为指定枚举类型的值,无效的枚举值 '{value}'，请检查输入并重试。");
            }

            return result;
        }
        /// <summary>
        /// 将字符串解析为指定枚举类型的值，可指定是否区分大小写。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <param name="value">要解析的字符串。</param>
        /// <param name="ignoreCase">是否忽略大小写。</param>
        /// <returns>解析后的枚举值。</returns>
        /// <exception cref="ArgumentException">当方法或构造函数的参数不符合预期的格式、类型或值时，将抛出ArgumentException异常。该异常通常用于检测并处理输入错误。</exception>
        public static T ParseEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("将字符串解析为指定枚举类型的值,值不能为空或空白。");
            }

            if (!Enum.TryParse<T>(value, ignoreCase, out T result))
            {
                throw new ArgumentException($"将字符串解析为指定枚举类型的值,无效的枚举值 '{value}'，请检查输入并重试。");
            }

            return result;
        }
        /// <summary>
        /// 将字符串解析为指定枚举类型的值，并根据指定的描述属性进行匹配。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <param name="value">要解析的字符串。</param>
        /// <param name="ignoreCase">是否忽略大小写。</param>
        /// <param name="getDescription">获取枚举值描述属性值的委托。</param>
        /// <returns>解析后的枚举值。</returns>
        /// <exception cref="ArgumentException">当方法或构造函数的参数不符合预期的格式、类型或值时，将抛出ArgumentException异常。该异常通常用于检测并处理输入错误。</exception>
        public static T ParseEnum<T>(this string value, bool ignoreCase, Func<T, string> getDescription) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("将字符串解析为指定枚举类型的值,值不能为空或空白。");
            }

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                string description = getDescription(enumValue);
                if (string.Equals(description, value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return enumValue;
                }
            }
            throw new ArgumentException($"将字符串解析为指定枚举类型的值,无效的枚举值 '{value}'，请检查输入并重试。");
        }
        /// <summary>
        /// 获取枚举值的Description特性中文注释
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举值的Description特性中文注释</returns>
        public static string GetDescription(this Enum e)
        {
            Type type = e.GetType();
            MemberInfo[] memberInfo = type.GetMember(e.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return e.ToString();
        }
    }
}

