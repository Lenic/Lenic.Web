using System;
using Lenic.Framework.Common;
using Lenic.Framework.Common.Logging;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 缓存容器接口扩展方法集合
    /// </summary>
    public static class ICacheContainerExtensions
    {
        #region Original Methods

        /// <summary>
        /// 安全地获取缓存容器中的值。
        /// </summary>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="defaultValue">获取失败后，要返回的缺省值。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key</exception>
        public static object GetValue(this ICacheContainer container, string key, object defaultValue = null)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetValue].container");
            if (key == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetValue].key");

            object value = null;
            if (container.TryGetValue(key, out value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// 安全地获取缓存容器中的值。
        /// </summary>
        /// <typeparam name="TResult">缓存容器值的强类型表示。</typeparam>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="defaultValue">获取失败后，要返回的缺省值。</param>
        /// <param name="throwIfException">如果设置为 <c>true</c> 表示错误后抛出异常。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key</exception>
        public static TResult GetValueX<TResult>(this ICacheContainer container, string key, TResult defaultValue = default(TResult), bool throwIfException = true)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetValueX<{0}>].container", typeof(TResult).FullName));
            if (key == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetValueX<{0}>].key", typeof(TResult).FullName));

            try
            {
                object value = null;
                if (container.TryGetValue(key, out value))
                    return (TResult)value;

                return defaultValue;
            }
            catch
            {
                if (throwIfException)
                    throw;
                else
                    return defaultValue;
            }
        }

        /// <summary>
        /// 安全地获取缓存容器中的值。
        /// </summary>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="defaultValueFunc">获取失败后，要返回的缺省值委托。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or
        /// defaultValueFunc</exception>
        public static object GetValueWithFunc(this ICacheContainer container, string key, Func<object> defaultValueFunc)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetValueWithFunc].container");
            if (key == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetValueWithFunc].key");
            if (defaultValueFunc == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetValueWithFunc].defaultValueFunc");

            object value = null;
            if (container.TryGetValue(key, out value))
                return value;

            return defaultValueFunc();
        }

        /// <summary>
        /// 安全地获取缓存容器中的值。
        /// </summary>
        /// <typeparam name="TResult">缓存容器值的强类型表示。</typeparam>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="defaultValueFunc">获取失败后，要返回的缺省值委托。</param>
        /// <param name="throwIfException">如果设置为 <c>true</c> 表示错误后抛出异常。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or
        /// defaultValueFunc</exception>
        public static TResult GetValueXWithFunc<TResult>(this ICacheContainer container, string key, Func<TResult> defaultValueFunc, bool throwIfException = true)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetValueXWithFunc<{0}>].container", typeof(TResult).FullName));
            if (key == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetValueXWithFunc<{0}>].key", typeof(TResult).FullName));
            if (defaultValueFunc == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetValueXWithFunc<{0}>].defaultValueFunc", typeof(TResult).FullName));

            try
            {
                object value = null;
                if (container.TryGetValue(key, out value))
                    return (TResult)value;

                return defaultValueFunc();
            }
            catch
            {
                if (throwIfException)
                    throw;
                else
                    return defaultValueFunc();
            }
        }

        /// <summary>
        /// 根据指定键获取或添加指定值。
        /// </summary>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="newValue">获取失败后待添加的新值。</param>
        /// <param name="expiration">缓存项的过期设置信息：<c>null</c> 表示使用缺省的过期设置。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or newValue</exception>
        public static object GetOrAdd(this ICacheContainer container, string key, object newValue, Expiration expiration = null)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetOrAdd].container");
            if (key == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetOrAdd].key");

            object value = null;
            if (container.TryGetValue(key, out value))
                return value;

            container.Update(key, newValue, expiration);
            return newValue;
        }

        /// <summary>
        /// 根据指定键获取或添加指定值。
        /// </summary>
        /// <typeparam name="TResult">缓存容器值的强类型表示。</typeparam>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="newValue">获取失败后待添加的新值。</param>
        /// <param name="expiration">缓存项的过期设置信息：<c>null</c> 表示使用缺省的过期设置。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or newValue</exception>
        public static TResult GetOrAddX<TResult>(this ICacheContainer container, string key, TResult newValue, Expiration expiration = null)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddX<{0}>].container", typeof(TResult).FullName));
            if (key == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddX<{0}>].key", typeof(TResult).FullName));
            if (newValue == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddX<{0}>].newValue", typeof(TResult).FullName));

            object value = null;
            if (container.TryGetValue(key, out value))
                return (TResult)value;

            container.Update(key, newValue, expiration);
            return newValue;
        }

        /// <summary>
        /// 根据指定键获取或添加指定值。
        /// </summary>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="newValueFunc">获取失败后待添加的新值委托。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or
        /// newValueFunc</exception>
        public static object GetOrAddWithFunc(this ICacheContainer container, string key, Func<Tuple<object, Expiration>> newValueFunc)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetOrAddWithFunc].container");
            if (key == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetOrAddWithFunc].key");
            if (newValueFunc == null)
                throw new ArgumentNullException("[ICacheContainerExtensions].[GetOrAddWithFunc].newValueFunc");

            object value = null;
            if (container.TryGetValue(key, out value))
                return value;

            var newValue = newValueFunc();
            container.Update(key, newValue.Item1, newValue.Item2);
            return newValue.Item1;
        }

        /// <summary>
        /// 根据指定键获取或添加指定值。
        /// </summary>
        /// <typeparam name="TResult">缓存容器值的强类型表示。</typeparam>
        /// <param name="container">一个缓存容器接口的实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="newValueFunc">获取失败后待添加的新值委托。</param>
        /// <returns>获取得到的缓存项的值。</returns>
        /// <exception cref="System.ArgumentNullException">container or key or
        /// newValueFunc</exception>
        public static TResult GetOrAddXWithFunc<TResult>(this ICacheContainer container, string key, Func<Tuple<TResult, Expiration>> newValueFunc)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddXWithFunc<{0}>].container", typeof(TResult).FullName));
            if (key == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddXWithFunc<{0}>].key", typeof(TResult).FullName));
            if (newValueFunc == null)
                throw new ArgumentNullException(string.Format("[ICacheContainerExtensions].[GetOrAddXWithFunc<{0}>].newValueFunc", typeof(TResult).FullName));

            object value = null;
            if (container.TryGetValue(key, out value))
                return (TResult)value;

            var newValue = newValueFunc();
            container.Update(key, newValue.Item1, newValue.Item2);
            return newValue.Item1;
        }

        #endregion Original Methods

        #region Notification Extensions

        /// <summary>
        /// 根据指定的键更新缓存项的值。
        /// </summary>
        /// <typeparam name="T">待更新的缓存项的类型。</typeparam>
        /// <param name="router">包含有缓存项通知订阅的消息路由实例对象。</param>
        /// <param name="key">缓存项的键。</param>
        /// <param name="obj">待更新的缓存项目标值：<c>null</c> 表示清除缓存项。</param>
        /// <returns>一个表示异步操作状态的结果实例对象。</returns>
        public static IMessageRouter UpdateCache<T>(this IMessageRouter router, string key, T obj)
        {
            var objectChanged = new ObjectExtendible<T>();
            objectChanged.SetChange(ObjectChangeType.Update);
            objectChanged.SetParameter(obj);
            objectChanged.Tag["Key"] = key;

            router.Publish(objectChanged);

            return router;
        }

        #endregion Notification Extensions
    }
}