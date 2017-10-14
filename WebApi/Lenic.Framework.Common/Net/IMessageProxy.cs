using System;

namespace Lenic.Framework.Common.Net
{
    /// <summary>
    /// 远程消息代理接口
    /// </summary>
    public interface IMessageProxy
    {
        /// <summary>
        /// 在消息接收完毕时触发。
        /// </summary>
        event Action<Message> OnReceived;

        /// <summary>
        /// 开始接收消息，在接收到消息后会触发 OnReceived 事件。
        /// </summary>
        void Receive();

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="obj">要发送的消息。</param>
        void Send(Message obj);
    }
}