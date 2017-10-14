using System;
using System.Collections.Generic;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 消息订阅者类
    /// </summary>
    public class Subscriber : ISubscriber
    {
        #region Entrance

        /// <summary>
        /// 初始化新建一个消息订阅者类的实例对象。
        /// </summary>
        public Subscriber()
        {
            Tag = new Dictionary<string, object>();
        }

        #endregion Entrance

        #region ISubscriber 成员

        /// <summary>
        /// 获取或设置订阅者的处理逻辑。
        /// </summary>
        /// <value>
        /// 订阅者的处理逻辑。
        /// </value>
        public Action<EventContext> Action { get; set; }

        /// <summary>
        /// 获取或设置订阅者的消息筛选器。
        /// </summary>
        /// <value>
        /// 订阅者的消息筛选器。
        /// </value>
        public Func<object, bool> Filter { get; set; }

        /// <summary>
        /// 获取或设置消息订阅者的标识。
        /// </summary>
        /// <value>
        /// 消息订阅者的标识。
        /// </value>
        public object Identity { get; set; }

        /// <summary>
        /// 获取或设置订阅者的处理顺序。
        /// </summary>
        public double Index { get; set; }

        #endregion ISubscriber 成员

        #region ISubscriber 显示成员

        /// <summary>
        /// 获取或设置订阅者的处理逻辑。
        /// </summary>
        /// <value>
        /// 订阅者的处理逻辑。
        /// </value>
        Action<object> ISubscriber.Action
        {
            get { return p => Action(p as EventContext); }
            set { this.Action = p => value(p as EventContext); }
        }

        #endregion ISubscriber 显示成员

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