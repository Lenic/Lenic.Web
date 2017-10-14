using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 响应结果类
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ResponseResult
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置错误类型代码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 获取或设置执行过程中发生的错误信息。
        /// </summary>
        /// <value>执行过程中发生的错误信息。</value>
        public Exception Exception { get; set; }

        /// <summary>
        /// 获取或设置一个值，通过该值指示是否执行成功：<c>true</c> 表示执行成功。
        /// </summary>
        /// <value><c>true</c> 表示执行成功；否则返回 <c>false</c> 。</value>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 获取或设置响应消息的文本描述信息。
        /// </summary>
        /// <value>响应消息的文本描述信息。</value>
        public string Message { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="ResponseResult" /> 类的实例对象。
        /// </summary>
        public ResponseResult()
            : this(false, null)
        {
        }

        /// <summary>
        /// 初始化新建一个 <see cref="ResponseResult" /> 类的实例对象。
        /// </summary>
        /// <param name="result">如果设置为 <c>true</c> 表示执行成功。</param>
        /// <param name="message">响应消息的文本描述信息。</param>
        public ResponseResult(bool result, string message)
        {
            IsSuccess = result;
            Message = message ?? string.Empty;
            Tag = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 创建一个执行失败的响应消息。
        /// </summary>
        /// <param name="message">响应消息的文本描述信息。</param>
        /// <param name="errorCode">错误类型代码。</param>
        /// <returns>一个 <see cref="ResponseResult" /> 类的实例对象。</returns>
        public static ResponseResult NewFailure(string message, int errorCode = 0)
        {
            return new ResponseResult(false, message) { ErrorCode = errorCode };
        }

        /// <summary>
        /// 创建一个执行成功的响应消息。
        /// </summary>
        /// <param name="message">响应消息的文本描述信息。</param>
        /// <returns>一个 <see cref="ResponseResult" /> 类的实例对象。</returns>
        public static ResponseResult NewSuccess(string message = null)
        {
            return new ResponseResult(true, message);
        }

        #endregion Entrance

        #region IObjectExtendible 成员

        /// <summary>
        /// 获取响应消息中的扩展属性：键忽略大小写。
        /// </summary>
        public IDictionary<string, string> Tag { get; private set; }

        #endregion IObjectExtendible 成员
    }
}