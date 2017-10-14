using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 可迭代的枚举类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enum<T> : IEnumerable<KeyValuePair<string, T>> where T : struct
    {
        #region Private Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private static readonly Enum<T> _instance = new Enum<T>();

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private LinkedList<KeyValuePair<string, T>> _cache = new LinkedList<KeyValuePair<string, T>>();

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private Dictionary<string, T> _dic = null;

        #endregion Private Fields

        #region Entrance

        private Enum()
        {
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));

            for (int i = 0; i < names.Length; i++)
                _cache.AddLast(new KeyValuePair<string, T>(names[i], (T)values.GetValue(i)));

            _dic = _cache.ToDictionary(p => p.Key, p => p.Value);
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 返回类型为 System.Collections.Generic.IEnumerable&lt;T&gt; 的序列。
        /// </summary>
        /// <returns>类型为 System.Collections.Generic.IEnumerable&lt;T&gt; 的序列。</returns>
        public static IEnumerable<KeyValuePair<string, T>> AsEnumerable()
        {
            return _instance;
        }

        /// <summary>
        /// 根据枚举的字符串描述获取对应的枚举值。
        /// </summary>
        /// <param name="key">枚举的字符串描述信息。</param>
        /// <returns>描述信息对应的枚举值，如果不存在则返回 <c>null</c> 。</returns>
        public static T? GetValue(string key)
        {
            T result = default(T);
            if (_instance._dic.TryGetValue(key, out result))
                return result;
            else
                return null;
        }

        #endregion Business Methods

        #region Interface Members

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var item in _cache)
                yield return item;
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion Interface Members
    }
}