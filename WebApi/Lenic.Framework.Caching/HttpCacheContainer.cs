using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// HttpCache 缓存容器
    /// </summary>
    public class HttpCacheContainer : BaseCacheContainer
    {
        #region Private Fields

        private Cache _cache = HttpRuntime.Cache;

        #endregion Private Fields

        #region Protected Methods

        /// <summary>
        /// 根据指定的键的集合获取值的集合。
        /// </summary>
        /// <param name="keys">待获取值的键的集合。</param>
        /// <returns>获取得到的值的集合。</returns>
        public override object[] GetValues(IEnumerable<string> keys)
        {
            return keys.Select(p => _cache.Get(GetKey(p))).ToArray();
        }

        /// <summary>
        /// 移除指定键的缓存项。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        public override void Remove(string key)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("[HttpCacheContainer].[Remove].key");

            _cache.Remove(GetKey(key));
        }

        /// <summary>
        /// 尝试获取指定键的值。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">输出的缓存项的值，在返回值为 <c>true</c> 时表示获取成功。</param>
        /// <returns><c>true</c> 表示成功，值为输出参数
        /// <paramref name="value" />；否则返回 <c>false</c> 。</returns>
        public override bool TryGetValue(string key, out object value)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("[HttpCacheContainer].[TryGetValue].key");

            return !ReferenceEquals(value = _cache.Get(GetKey(key)), null);
        }

        /// <summary>
        /// 将缓存项保存到缓存容器中。
        /// </summary>
        /// <param name="cacheItemKey">用于缓存项的键名称。</param>
        /// <param name="methodName">用户调用的原始方法名称：Add 或者 Update</param>
        /// <param name="expiration">与之关联的缓存过期信息。</param>
        protected override void Push(string cacheItemKey, string methodName, Expiration expiration)
        {
            var absoluteExpiration = !expiration.AutoDelay ? expiration.DeadTime : Cache.NoAbsoluteExpiration;
            var slidingExpiration = expiration.AutoDelay ? (expiration.DeadTime - expiration.InitialTime) : Cache.NoSlidingExpiration;

            if (methodName == "Add")
            {
                var obj = _cache.Add(cacheItemKey, expiration.Notification.Value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
                if (!ReferenceEquals(obj, null))
                    throw new InvalidOperationException(string.Format("已存在相同键【{0}】的项！", expiration.Notification.Key));
            }
            else
                _cache.Insert(cacheItemKey, expiration.Notification.Value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);

            expiration.Notification.SetValue(CacheNotification.DefaultValue, false);
        }

        #endregion Protected Methods
    }
}