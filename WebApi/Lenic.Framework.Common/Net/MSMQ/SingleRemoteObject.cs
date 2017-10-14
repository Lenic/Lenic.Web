using System;
using System.Messaging;

namespace Lenic.Framework.Common.Net.MSMQ
{
    /// <summary>
    /// 单个远程对象类
    /// </summary>
    public class SingleRemoteObject : IRemoteObject
    {
        #region Private Fields

        private MessageQueue _source = null;
        private MessageQueue _target = null;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="SingleRemoteObject"/> 类的实例对象。
        /// </summary>
        /// <param name="target">待发送的目标地址。</param>
        /// <param name="source">接受监听的地址路径。</param>
        /// <exception cref="System.ArgumentNullException">
        /// target
        /// or
        /// source
        /// </exception>
        public SingleRemoteObject(string target, string source)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            _source = MessageProxyCreater.CreateLocalQueue(source);
            _target = MessageProxyCreater.CreateRemoteQueue(target);
        }

        #endregion Entrance

        #region IReachable 成员

        /// <summary>
        /// 接收远程对象的消息（必须阻塞线程直到收到消息）。
        /// </summary>
        /// <returns>来自远程对象的消息。</returns>
        public Message Receive()
        {
            var msg = _source.Receive();
            msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(Message) });

            return (Message)msg.Body;
        }

        /// <summary>
        /// 向远程对象发送消息。
        /// </summary>
        /// <param name="msg">待发送的消息对象。</param>
        public void Send(Message msg)
        {
            var obj = new System.Messaging.Message();

            obj.Body = msg;
            obj.Formatter = new XmlMessageFormatter(new Type[] { typeof(Message) });

            _target.Send(obj, Environment.MachineName);
        }

        #endregion IReachable 成员
    }
}