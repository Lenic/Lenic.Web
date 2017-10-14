using System;
using System.Runtime.Serialization;
using System.Security;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 业务层面的异常信息类
    /// </summary>
    [Serializable]
    public class BusinessException : Exception
    {
        #region Entrance

        /// <summary>
        /// 初始化 <see cref="BusinessException" /> 类的新实例。
        /// </summary>
        public BusinessException()
            : base()
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="BusinessException" /> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public BusinessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="BusinessException" /> 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="BusinessException" /> 类的新实例。
        /// </summary>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public BusinessException(Exception innerException)
            : this(string.Format("包装内层错误信息：{0}", (innerException == null ? null : innerException.Message)), innerException)
        {
        }

        /// <summary>
        /// 用序列化数据初始化 <see cref="BusinessException" /> 类的新实例。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" />，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" />，它包含有关源或目标的上下文信息。</param>
        [SecuritySafeCritical]
        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Entrance
    }
}