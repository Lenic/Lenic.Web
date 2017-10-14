using System;

namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    [Flags]
    public enum LogTypes
    {
        /// <summary>
        /// 【缺省】程序员自定义的调试信息，此类型为最低级别：0
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 一些信息性的消息，此类别仅比 Debug 稍高一级：1
        /// </summary>
        Info = 2,

        /// <summary>
        /// 程序中的一些警告性信息，不影响程序运行，但是建议修改：2
        /// </summary>
        Warn = 4,

        /// <summary>
        /// 程序运行的错误，会导致程序抛出异常：3
        /// </summary>
        Error = 8,

        /// <summary>
        /// 导致程序出现崩溃性错误的问题，此级别为最高级别：4
        /// </summary>
        Fatal = 16,
    }
}