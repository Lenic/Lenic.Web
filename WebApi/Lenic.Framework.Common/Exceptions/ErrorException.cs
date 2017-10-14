using System;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 错误级别异常类
    /// </summary>
    [Serializable]
    public class ErrorException : LevelException
    {
        #region Entrance

        /// <summary>
        /// 初始化 <see cref="ErrorException" /> 类的新实例。
        /// </summary>
        public ErrorException()
            : base()
        {
            Level = ExceptionLevel.Error;
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="ErrorException" /> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public ErrorException(string message)
            : base(message)
        {
            Level = ExceptionLevel.Error;
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="ErrorException" /> 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public ErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
            Level = ExceptionLevel.Error;
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="ErrorException" /> 类的新实例。
        /// </summary>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public ErrorException(Exception innerException)
            : this(string.Format("包装内层错误信息：{0}", (innerException == null ? null : innerException.Message)), innerException)
        {
        }

        #endregion Entrance
    }
}