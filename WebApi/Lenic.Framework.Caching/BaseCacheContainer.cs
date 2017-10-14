using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 缓存容器虚基类
    /// </summary>
    [DebuggerStepThrough]
    public abstract class BaseCacheContainer : ICacheContainer
    {
        #region Private Fields

        private bool _isDisposed = false;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取当前类型的名称（简写名称）。
        /// </summary>
        protected string CurrentTypeName { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="BaseCacheContainer"/> 类的实例对象。
        /// </summary>
        protected BaseCacheContainer()
        {
            CurrentTypeName = GetType().Name;
        }

        #endregion Entrance

        #region Protected Methods

        /// <summary>
        /// 通过原始键获取在缓存容器中项的键。
        /// </summary>
        /// <param name="originalKey">缓存项的原始键。</param>
        /// <returns>封装后的缓存容器中项的键。</returns>
        protected internal string GetKey(string originalKey)
        {
            return string.Format("{0}_{1}", CurrentTypeName, originalKey);
        }

        /// <summary>
        /// 将缓存项保存到缓存容器中。
        /// </summary>
        /// <param name="cacheItemKey">用于缓存项的键名称。</param>
        /// <param name="methodName">用户调用的原始方法名称：Add 或者 Update</param>
        /// <param name="expiration">与之关联的缓存过期信息。</param>
        protected abstract void Push(string cacheItemKey, string methodName, Expiration expiration);

        /// <summary>
        /// 设置缓存项的值。
        /// </summary>
        /// <param name="key">缓存项的原始键值。</param>
        /// <param name="value">缓存项的值。</param>
        /// <param name="methodName">调用当前的方法的方法名称。</param>
        /// <param name="expiration">缓存项的过期信息实例对象：<c>null</c> 表示滑动十分钟过期。</param>
        protected void SetValue(string key, object value, string methodName, Expiration expiration)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("[BaseCacheContainer].[SetValue].key");

            if (ReferenceEquals(value, null))
                return;

            expiration = expiration ?? Expiration.CreateInstance();
            if (expiration.Container != null && !ReferenceEquals(expiration.Container, this))
                throw new ArgumentException("[BaseCacheContainer].[SetValue].expiration.Container 已存在，不能添加！");
            expiration.Container = this;

            var notification = expiration.Notification ?? CacheNotification.CreateInstance();
            notification.Key = key;
            notification.SetValue(value, false);

            if (notification.Expiration != null && !ReferenceEquals(notification.Expiration, expiration))
                throw new ArgumentException("[BaseCacheContainer].[SetValue].expiration.[Notification].Expiration 已存在，过期对象处理错误！");
            notification.Expiration = expiration;

            Push(GetKey(key), methodName, expiration);
        }

        #endregion Protected Methods

        #region IDisposable 成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (!_isDisposed)
                {
                    OnDisposing();
                    _isDisposed = true;
                }
            }
        }

        /// <summary>
        /// 在请求释放资源时触发。
        /// </summary>
        protected virtual void OnDisposing()
        {
        }

        #endregion IDisposable 成员

        #region ICacheContainer 成员

        /// <summary>
        /// 向缓存容器中添加一个新的项。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">待缓存的值。</param>
        /// <param name="expiration">缓存项的过期设置信息，<c>null</c> 表示滑动十分钟过期。</param>
        public void Add(string key, object value, Expiration expiration = null)
        {
            SetValue(key, value, "Add", expiration);
        }

        /// <summary>
        /// 根据指定的键的集合获取值的集合。
        /// </summary>
        /// <param name="keys">待获取值的键的集合。</param>
        /// <returns>获取得到的值的集合。</returns>
        public abstract object[] GetValues(IEnumerable<string> keys);

        /// <summary>
        /// 移除指定键的缓存项。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        public abstract void Remove(string key);

        /// <summary>
        /// 尝试获取指定键的值。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">输出的缓存项的值，在返回值为 <c>true</c> 时表示获取成功。</param>
        /// <returns><c>true</c> 表示成功，值为输出参数 <paramref name="value" />；否则返回 <c>false</c> 。</returns>
        public abstract bool TryGetValue(string key, out object value);

        /// <summary>
        /// 更新指定键的值。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="value">缓存项新的值。</param>
        /// <param name="expiration">缓存项的过期设置信息：<c>null</c> 表示滑动十分钟过期。</param>
        public void Update(string key, object value, Expiration expiration = null)
        {
            SetValue(key, value, "Update", expiration);
        }

        #endregion ICacheContainer 成员
    }
}