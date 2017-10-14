using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// IEnumerable 扩展方法集合
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 对 data 的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">data 集合的元素类型</typeparam>
        /// <param name="data">要进行迭代的元素集合。</param>
        /// <param name="action">要对 data 的每个元素执行的 System.Actionlt;Tgt; 委托。</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            foreach (var item in data)
            {
                action(item);
            }
        }

        /// <summary>
        /// 对 data 的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">data 集合的元素类型</typeparam>
        /// <param name="data">要进行迭代的元素集合。</param>
        /// <param name="action">要对 data 的每个元素执行的 System.Actionlt;T, intgt; 委托。</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T, int> action)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            foreach (var item in data)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// 安全地获取字典中的值.
        /// </summary>
        /// <typeparam name="TKey">字典中键的类型.</typeparam>
        /// <typeparam name="TValue">字典中值的类型.</typeparam>
        /// <param name="dic">待获取值的字典.</param>
        /// <param name="key">一个键，用于获取其在字典中的值.</param>
        /// <param name="defaultValue">获取失败时，返回的缺省值.</param>
        /// <returns>
        /// 获取的字典中的值.
        /// </returns>
        public static TValue GetValueX<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue)
        {
            if (dic == null)
                return defaultValue;

            TValue result = defaultValue;
            if (dic.TryGetValue(key, out result))
                return result;
            else
                return defaultValue;
        }

        /// <summary>
        /// 安全地获取列表中的值.
        /// </summary>
        /// <typeparam name="TValue">字典中值的类型.</typeparam>
        /// <param name="data">待获取值的列表.</param>
        /// <param name="index">索引值，用于获取其在列表中的值.</param>
        /// <param name="defaultValue">获取失败时，返回的缺省值.</param>
        /// <returns>
        /// 获取的列表中的值.
        /// </returns>
        public static TValue GetValueX<TValue>(this IList<TValue> data, int index, TValue defaultValue)
        {
            if (data == null || index > (data.Count - 1))
                return defaultValue;

            return data[index];
        }
    }
}