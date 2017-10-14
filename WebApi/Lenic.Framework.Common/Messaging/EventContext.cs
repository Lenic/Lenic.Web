using System;
using System.Collections.Generic;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 订阅事件执行上下文
    /// </summary>
    public sealed class EventContext : IObjectExtendible
    {
        #region Business Properties

        /// <summary>
        /// 获取上下文执行过程中发生的错误信息。
        /// </summary>
        /// <value>
        /// 上下文执行过程中发生的错误信息。
        /// </value>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 获取或设置一个值，通过该值指示是否继续执行后续的订阅事件：<c>true</c> 表示停止执行。
        /// </summary>
        public bool IsStopped { get; set; }

        /// <summary>
        /// 获取或设置要发布的消息对象。
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// 获取订阅事件执行上下文相关的扩展信息。
        /// </summary>
        public IDictionary<string, object> Tag { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个订阅事件执行上下文的实例对象。
        /// </summary>
        /// <param name="msg"></param>
        public EventContext(object msg)
        {
            Object = msg;
            Tag = new Dictionary<string, object>();
        }

        #endregion Entrance

        #region Business Properties

        /// <summary>
        /// 获取强类型的消息对象。
        /// </summary>
        /// <typeparam name="T">消息对象的显式类型。</typeparam>
        /// <returns>强类型的消息对象。</returns>
        public T GetObject<T>()
        {
            return (T)this.Object;
        }

        #endregion Business Properties
    }
}