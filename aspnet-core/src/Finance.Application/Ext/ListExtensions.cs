using Finance.PriceEval.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Extensions;
using System.Threading.Tasks;
using System.Reflection;

namespace Finance.Ext
{
    public static class ListExtensions
    {
        /// <summary>
        /// List集合中的所有类的所有元素保留两位小数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void Round2<T>(this List<T> list) where T : class
        {
            list.ForEach(p => p.Round2());
        }

        /// <summary>
        /// 类中的所有元素都保留两位小数保留两位小数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static void Round2<T>(this T item) where T : class
        {
            var typeDecimal = typeof(decimal);
            var typeDouble = typeof(double);


            var properties =
                item.GetType().GetProperties()
                .Where(p => p.PropertyType == typeDecimal || p.PropertyType == typeDouble);
            foreach (var propertie in properties)
            {
                if (propertie.PropertyType == typeDecimal)
                {
                    propertie.SetValue(item, Math.Round(propertie.GetValue(item).To<decimal>(), 2));
                }
                if (propertie.PropertyType == typeDouble)
                {
                    propertie.SetValue(item, Math.Round(propertie.GetValue(item).To<double>(), 2));
                }
            }
        }
        /// <summary>
        /// 判断两个集合是否是相等的(所有的元素及数量都相等)
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="sourceCollection">源集合列表</param>
        /// <param name="targetCollection">目标集合列表</param>
        /// <returns>两个集合相等则返回True,否则返回False</returns>
        public static bool EqualList<T>(this IList<T> sourceCollection, IList<T> targetCollection) where T : IEquatable<T>
        {
            //空集合直接返回False,即使是两个都是空集合,也返回False
            if (sourceCollection == null || targetCollection == null)
            {
                return false;
            }

            if (object.ReferenceEquals(sourceCollection, targetCollection))
            {
                return true;
            }

            if (sourceCollection.Count != targetCollection.Count)
            {
                return false;
            }

            var sourceCollectionStaticsDict = sourceCollection.StatisticRepetition();
            var targetCollectionStaticsDict = targetCollection.StatisticRepetition();

            return sourceCollectionStaticsDict.EqualDictionary(targetCollectionStaticsDict);
        }

        /// <summary>
        /// 判断两个字典是否是相等的(所有的字典项对应的值都相等)
        /// </summary>
        /// <typeparam name="TKey">字典项类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="sourceDictionary">源字典</param>
        /// <param name="targetDictionary">目标字典</param>
        /// <returns>两个字典相等则返回True,否则返回False</returns>
        public static bool EqualDictionary<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary, Dictionary<TKey, TValue> targetDictionary)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue>
        {
            //空字典直接返回False,即使是两个都是空字典,也返回False
            if (sourceDictionary == null || targetDictionary == null)
            {
                return false;
            }

            if (object.ReferenceEquals(sourceDictionary, targetDictionary))
            {
                return true;
            }

            if (sourceDictionary.Count != targetDictionary.Count)
            {
                return false;
            }

            //比较两个字典的Key与Value
            foreach (var item in sourceDictionary)
            {
                //如果目标字典不包含源字典任意一项,则不相等
                if (!targetDictionary.ContainsKey(item.Key))
                {
                    return false;
                }

                //如果同一个字典项的值不相等,则不相等
                if (!targetDictionary[item.Key].Equals(item.Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 统计集合的重复项,并返回一个字典
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="sourceCollection">统计集合列表</param>
        /// <returns>返回一个集合元素及重复数量的字典</returns>
        private static Dictionary<T, int> StatisticRepetition<T>(this IEnumerable<T> sourceCollection) where T : IEquatable<T>
        {
            var collectionStaticsDict = new Dictionary<T, int>();
            foreach (var item in sourceCollection)
            {
                if (collectionStaticsDict.ContainsKey(item))
                {
                    collectionStaticsDict[item]++;
                }
                else
                {
                    collectionStaticsDict.Add(item, 1);
                }
            }

            return collectionStaticsDict;
        }
    }
}
