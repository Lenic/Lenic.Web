using System;
using System.Collections.Generic;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 异步消息订阅者类
    /// </summary>
    public class AsyncSubscriber : ISubscriber
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置异步执行的句柄。
        /// </summary>
        /// <value>
        /// 异步执行的句柄。
        /// </value>
        public IAsyncResult AsyncHander { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个异步消息订阅者类的实例对象。。
        /// </summary>
        public AsyncSubscriber()
        {
        }

        #endregion Entrance

        #region ISubscriber 成员

        /// <summary>
        /// 获取或设置消息订阅者的标识。
        /// </summary>
        /// <value>
        /// 消息订阅者的标识。
        /// </value>
        public object Identity { get; set; }

        /// <summary>
        /// 获取或设置订阅者的处理逻辑。
        /// </summary>
        /// <value>
        /// 订阅者的处理逻辑。
        /// </value>
        public Action<object> Action { get; set; }

        /// <summary>
        /// 获取或设置订阅者的消息筛选器。
        /// </summary>
        /// <value>
        /// 订阅者的消息筛选器。
        /// </value>
        public Func<object, bool> Filter { get; set; }

        /// <summary>
        /// 获取或设置订阅者的处理顺序。
        /// </summary>
        /// <value>
        /// 订阅者的处理顺序。
        /// </value>
        public double Index { get; set; }

        #endregion ISubscriber 成员

        #region IObjectExtendible 成员

        /// <summary>
        /// 获取消息订阅类相关的扩展信息。
        /// </summary>
        /// <value>
        /// 对象扩展属性字典集合。
        /// </value>
        public IDictionary<string, object> Tag { get; private set; }

        #endregion IObjectExtendible 成员
    }
}