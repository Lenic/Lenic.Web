using System;
using System.Diagnostics;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 日志信息扩展方法集合
    /// </summary>
    [DebuggerStepThrough]
    public static class LogEntryExtensions
    {
        /// <summary>
        /// 获取一个日志对象，并开始记录日志。
        /// </summary>
        /// <param name="router">一个消息路由接口的实例对象。</param>
        /// <param name="errorCode">错误类别代码。</param>
        /// <returns>初始化完成的一个日志对象。</returns>
        public static LogEntry NewLog(this IMessageRouter router, int errorCode = 0)
        {
            if (router == null)
                throw new ArgumentNullException("[LogInfoExtensions].[Log].router");

            return new LogEntry { ErrorCode = errorCode, Router = router };
        }

        /// <summary>
        /// 记录并发布一个 <see cref="LogTypes.Debug"/> 级别的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Debug(this LogEntry log, string description)
        {
            return Write(log, LogTypes.Debug, description);
        }

        /// <summary>
        /// 记录并发布一个 <see cref="LogTypes.Info"/> 级别的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Info(this LogEntry log, string description)
        {
            return Write(log, LogTypes.Info, description);
        }

        /// <summary>
        /// 记录并发布一个 <see cref="LogTypes.Warn"/> 级别的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Warn(this LogEntry log, string description)
        {
            return Write(log, LogTypes.Warn, description);
        }

        /// <summary>
        /// 记录并发布一个 <see cref="LogTypes.Error"/> 级别的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Error(this LogEntry log, string description)
        {
            return Write(log, LogTypes.Error, description);
        }

        /// <summary>
        /// 记录并发布一个 <see cref="LogTypes.Fatal"/> 级别的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Fatal(this LogEntry log, string description)
        {
            return Write(log, LogTypes.Fatal, description);
        }

        /// <summary>
        /// 记录并发布一个 <see cref="Lenic.Framework.Common.Logging.LogEntry"/> 类型的日志。
        /// </summary>
        /// <param name="log">初始化完成的一个日志对象。</param>
        /// <param name="logType">错误类别代码。</param>
        /// <param name="description">文本描述性信息。</param>
        /// <returns>已完成记录的日志对象：如果 Router 属性未被初始化，则不会被记录。</returns>
        public static LogEntry Write(this LogEntry log, LogTypes logType, string description)
        {
            log.LogType = logType;
            log.Description = description ?? string.Empty;

            if (log.Router != null)
                log.Router.Publish(log);

            return log;
        }
    }
}