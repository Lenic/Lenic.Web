using System;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 触发 <see cref="IMessageRouter"/> 的发布异常。
    /// </summary>
    /// <param name="sender">触发异常的原实例对象。</param>
    /// <param name="e">发布异常的具体信息。</param>
    public delegate void RaisePublishException(object sender, PublishExceptionArgs e);

    /// <summary>
    /// 发布异常参数类
    /// </summary>
    public class PublishExceptionArgs : EventArgs
    {
        /// <summary>
        /// 获取引发异常时的原始对象。
        /// </summary>
        public object Object { get; private set; }

        /// <summary>
        /// 获取发布引发的异常信息。
        /// </summary>
        public AggregateException Exception { get; private set; }

        internal PublishExceptionArgs(object obj, AggregateException exception)
        {
            Object = obj;
            Exception = exception;
        }
    }
}