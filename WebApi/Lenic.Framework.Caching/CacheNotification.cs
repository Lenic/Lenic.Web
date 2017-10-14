using System;
using System.Diagnostics;
using Lenic.Framework.Common;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 缓存项通知类
    /// </summary>
    [DebuggerStepThrough]
    public class CacheNotification
    {
        #region Fields

        /// <summary>
        /// 执行 Notify 方法后的 Value 属性的值。
        /// </summary>
        public static readonly string DefaultValue = "_$CacheNotification.DefaultValue$_";

        #endregion Fields

        #region Business Properties

        /// <summary>
        /// 获取或设置缓存项的值（Value）是否已经发生变更：<c>true</c> 表示已发生了变更。
        /// </summary>
        protected internal bool IsChanged { get; set; }

        /// <summary>
        /// 获取移除通知关联的过期设置信息实例对象。
        /// </summary>
        public Expiration Expiration { get; internal set; }

        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取缓存项的值：当前属性值为 <c>null</c> 时执行通知表示移除缓存项，否则表示更新缓存项的值。
        /// </summary>
        public object Value { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 创建一个缺省的缓存项通知类的实例对象。
        /// </summary>
        public static CacheNotification CreateInstance()
        {
            return new CacheNotification();
        }

        /// <summary>
        /// 创建一个附带消息订阅的缓存通知实例对象。
        /// </summary>
        /// <typeparam name="T">需要监听的消息实体类型</typeparam>
        /// <param name="router">当前正在使用的消息路由实例对象。</param>
        /// <returns>一个缓存项移除通知的实例对象。</returns>
        public static CacheNotification CreateInstance<T>(IMessageRouter router)
        {
            return new KeyMessageNotification<T>(router);
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 执行通知。
        /// </summary>
        public void Notify()
        {
            if (Key == null)
                throw new ArgumentNullException("[RemoveNotification].[Key]");
            if (Expiration == null)
                throw new ArgumentNullException("[RemoveNotification].[Expiration]");
            if (Expiration.Container == null)
                throw new ArgumentNullException("[RemoveNotification].[Expiration].Container");

            try
            {
                ExecuteNotifyCore();

                if (IsChanged)
                {
                    lock (this)
                    {
                        if (ReferenceEquals(Value, null))
                            Expiration.Container.Remove(Key);
                        else
                        {
                            Expiration.Container.Update(Key, Value, Expiration);
                            SetValue(CacheNotification.DefaultValue, false);
                        }

                        IsChanged = false;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 设置缓存项的值。
        /// </summary>
        /// <param name="value">缓存项的值。</param>
        /// <param name="isChanged">如果设置为 <c>true</c> 表示执行 Notify 方法时检查是否需要移除缓存项，默认值为 <c>true</c> 。</param>
        public void SetValue(object value, bool isChanged = true)
        {
            lock (this)
            {
                Value = value;
                IsChanged = isChanged;
            }
        }

        #endregion Business Methods

        #region Virtual Methods

        /// <summary>
        /// 执行通知的核心方法：当前属性 Value 值为 <c>null</c> 时执行通知表示移除缓存项，否则表示更新缓存项的值。
        /// </summary>
        protected virtual void ExecuteNotifyCore()
        {
            SetValue(null);
        }

        #endregion Virtual Methods
    }
}