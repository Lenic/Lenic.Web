using System;
using System.Diagnostics;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 缓存过期设置信息类
    /// </summary>
    [DebuggerStepThrough]
    public sealed class Expiration
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置一个值，通过该值指示是否自增长：<c>true</c> 表示滑动过期。
        /// </summary>
        /// <value><c>true</c> 表示滑动过期；否则表示绝对过期。</value>
        public bool AutoDelay { get; set; }

        /// <summary>
        /// 获取过期设置信息关联的缓存容器实例。
        /// </summary>
        public ICacheContainer Container { get; internal set; }

        /// <summary>
        /// 获取或设置第一次过期的时间点。
        /// </summary>
        public DateTime DeadTime { get; set; }

        /// <summary>
        /// 获取当前实例的创建时间。
        /// </summary>
        public DateTime InitialTime { get; private set; }

        /// <summary>
        /// 获取或设置过期信息关联的过期通知信息。
        /// </summary>
        public CacheNotification Notification { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="Expiration"/> 类的实例对象。
        /// </summary>
        public Expiration()
        {
            InitialTime = DateTime.Now;
        }

        /// <summary>
        /// 创建一个滑动十分钟过期的过期设置信息。
        /// </summary>
        public static Expiration CreateInstance()
        {
            return FromSliding(TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 从一个绝对时间点创建一个缓存过期设置。
        /// </summary>
        /// <param name="time">过期时间点。</param>
        /// <param name="notification">缓存过期通知信息：<c>null</c> 表示无通知。</param>
        /// <returns>一个缓存过期设置信息的实例对象。</returns>
        public static Expiration FromAbsolute(DateTime time, CacheNotification notification = null)
        {
            return new Expiration
            {
                DeadTime = time,
                AutoDelay = false,
                Notification = notification ?? CacheNotification.CreateInstance(),
            };
        }

        /// <summary>
        /// 从一个时间间隔创建一个缓存过期设置。
        /// </summary>
        /// <param name="time">从当前到过期的时间间隔。</param>
        /// <param name="notification">缓存过期通知信息：<c>null</c> 表示无通知。</param>
        /// <returns>一个缓存过期设置信息的实例对象。</returns>
        public static Expiration FromSliding(TimeSpan span, CacheNotification notification = null)
        {
            return new Expiration
            {
                DeadTime = DateTime.Now.Add(span),
                AutoDelay = true,
                Notification = notification ?? CacheNotification.CreateInstance(),
            };
        }

        #endregion Entrance
    }
}