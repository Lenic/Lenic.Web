namespace Lenic.Framework.Common.Net
{
    /// <summary>
    /// 远程对象接口
    /// </summary>
    public interface IRemoteObject
    {
        /// <summary>
        /// 向远程对象发送消息。
        /// </summary>
        /// <param name="msg">待发送的消息对象。</param>
        void Send(Message msg);

        /// <summary>
        /// 接收远程对象的消息（必须阻塞线程直到收到消息）。
        /// </summary>
        /// <returns>来自远程对象的消息。</returns>
        Message Receive();
    }
}