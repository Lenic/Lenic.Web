using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;

namespace Lenic.Framework.Common.Net.MSMQ
{
    /// <summary>
    /// 多重远程对象类
    /// </summary>
    public class MultiRemoteObject : IRemoteObject
    {
        #region Private Fields

        private MessageQueue _source = null;
        private string _template = null;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取订阅的客户端列表。
        /// </summary>
        /// <value>
        /// 订阅的客户端列表。
        /// </value>
        public IList<string> Clients { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="MultiRemoteObject" /> 类的实例对象。
        /// </summary>
        /// <param name="source">本地消息队列的名称。</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public MultiRemoteObject(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            _source = MessageProxyCreater.CreateLocalQueue(string.Format(@".\private$\{0}", source));
            _template = string.Concat(@"FormatName:DIRECT=TCP:{0}\private$\", source);

            Clients = new List<string>();
        }

        #endregion Entrance

        #region IRemoteObject 成员

        /// <summary>
        /// 向远程对象发送消息。
        /// </summary>
        /// <param name="msg">待发送的消息对象。</param>
        public void Send(Message msg)
        {
            var obj = new System.Messaging.Message();

            obj.Body = msg;
            obj.Formatter = new XmlMessageFormatter(new Type[] { typeof(Message) });

            var data = Clients.Select(p => MessageProxyCreater.CreateRemoteQueue(string.Format(_template, p)));
            foreach (var item in data)
            {
                item.Send(obj, Environment.MachineName);
            }
        }

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

        #endregion IRemoteObject 成员
    }
}