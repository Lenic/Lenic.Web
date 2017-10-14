using System;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 消息订阅者接口
    /// </summary>
    public interface ISubscriber : IObjectExtendible
    {
        /// <summary>
        /// 获取或设置订阅者的处理逻辑。
        /// </summary>
        /// <value>
        /// 订阅者的处理逻辑。
        /// </value>
        Action<object> Action { get; set; }

        /// <summary>
        /// 获取或设置订阅者的消息筛选器。
        /// </summary>
        /// <value>
        /// 订阅者的消息筛选器。
        /// </value>
        Func<object, bool> Filter { get; set; }

        /// <summary>
        /// 获取或设置消息订阅者的标识。
        /// </summary>
        /// <value>
        /// 消息订阅者的标识。
        /// </value>
        object Identity { get; set; }

        /// <summary>
        /// 获取或设置订阅者的处理顺序。
        /// </summary>
        /// <value>
        /// 订阅者的处理顺序。
        /// </value>
        double Index { get; set; }
    }
}