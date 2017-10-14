using System.Diagnostics;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 日志实体类
    /// </summary>
    [DebuggerStepThrough]
    public class LogEntry
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置错误类别，默认为 0 。
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 获取或设置日志信息级别，默认为 Debug 。
        /// </summary>
        public virtual LogTypes LogType { get; set; }

        /// <summary>
        /// 获取或设置日志的文本描述性信息，默认为 string.Empty 。
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 获取或设置当前日志实体的扩展信息。
        /// </summary>
        public virtual object Tag { get; set; }

        /// <summary>
        /// 获取或设置消息转发中心实力对象。
        /// </summary>
        protected internal IMessageRouter Router { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="LogEntry"/> 类的实例对象。
        /// </summary>
        public LogEntry()
        {
            LogType = LogTypes.Debug;
            Description = string.Empty;
        }

        /// <summary>
        /// 初始化新建一个 <see cref="LogEntry"/> 类的实例对象。
        /// </summary>
        /// <param name="logType">日志级别枚举。</param>
        /// <param name="description">文本描述性信息。</param>
        public LogEntry(LogTypes logType, string description)
        {
            LogType = logType;
            Description = description ?? string.Empty;
        }

        #endregion Entrance
    }
}