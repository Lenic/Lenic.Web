using System;
using System.Collections.Generic;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 缓存容器接口
    /// </summary>
    public interface ICacheContainer : IDisposable
    {
        /// <summary>
        /// 向缓存容器中添加一个新的项。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">待缓存的值。</param>
        /// <param name="expiration">缓存项的过期设置信息，<c>null</c> 表示滑动十分钟过期。</param>
        void Add(string key, object value, Expiration expiration = null);

        /// <summary>
        /// 根据指定的键的集合获取值的集合。
        /// </summary>
        /// <param name="keys">待获取值的键的集合。</param>
        /// <returns>获取得到的值的集合。</returns>
        object[] GetValues(IEnumerable<string> keys);

        /// <summary>
        /// 移除指定键的缓存项。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        void Remove(string key);

        /// <summary>
        /// 尝试获取指定键的值。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">输出的缓存项的值，在返回值为 <c>true</c> 时表示获取成功。</param>
        /// <returns><c>true</c> 表示成功，值为输出参数
        /// <paramref name="value"/>；否则返回 <c>false</c> 。</returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// 更新指定键的值。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">缓存项新的值。</param>
        /// <param name="expiration">缓存项的过期设置信息：<c>null</c> 表示沿用添加时的过期设置；否则表示更新过期设置。</param>
        void Update(string key, object value, Expiration expiration = null);
    }
}