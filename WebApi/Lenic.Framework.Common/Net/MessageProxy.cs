using System;

namespace Lenic.Framework.Common.Net
{
    /// <summary>
    /// 远程消息代理
    /// </summary>
    public class MessageProxy : IMessageProxy
    {
        #region Private Fields

        private IRemoteObject _proxy = null;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="MessageProxy"/> 类的实例对象。
        /// </summary>
        /// <param name="obj">一个远程可达的实例对象。</param>
        public MessageProxy(IRemoteObject obj)
        {
            _proxy = obj;
        }

        #endregion Entrance

        #region IMessageProxy 成员

        /// <summary>
        /// 在消息接收完毕时触发。
        /// </summary>
        public event Action<Message> OnReceived;

        /// <summary>
        /// 开始接收消息，在接收到消息后会触发 OnReceived 事件。
        /// </summary>
        public void Receive()
        {
            Action action = ReceiveLogic;

            action.BeginInvoke(null, null);
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="obj">要发送的消息。</param>
        public void Send(Message obj)
        {
            _proxy.Send(obj);
        }

        #endregion IMessageProxy 成员

        #region Private Methods

        private void ReceiveLogic()
        {
            var data = _proxy.Receive();

            if (OnReceived != null)
                OnReceived(data);
        }

        #endregion Private Methods
    }
}