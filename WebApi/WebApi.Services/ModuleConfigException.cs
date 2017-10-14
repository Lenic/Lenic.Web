using System;

namespace Lenic.Web.WebApi
{
    /// <summary>
    /// 框架配置异常类
    /// </summary>
    public class ModuleConfigException : Exception
    {
        /// <summary>
        /// 初始化新建一个 <see cref="ModuleConfigException"/> 类的实例对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public ModuleConfigException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化新建一个 <see cref="ModuleConfigException"/> 类的实例对象。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用（在 Visual Basic 中为
        /// Nothing）。</param>
        public ModuleConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}