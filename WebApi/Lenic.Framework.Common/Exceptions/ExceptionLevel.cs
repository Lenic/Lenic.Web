using System;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 异常信息等级
    /// </summary>
    [Serializable]
    public enum ExceptionLevel
    {
        /// <summary>
        /// 【缺省值】跟踪级别，记录全部异常信息。
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 调试级别，记录调试级别的异常信息。
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 提示级别，记录需要提示的异常信息。
        /// </summary>
        Info = 2,

        /// <summary>
        /// 警告级别，记录警告用户的异常信息。
        /// </summary>
        Warning = 3,

        /// <summary>
        /// 错误级别，记录系统错误的异常信息。
        /// </summary>
        Error = 4,

        /// <summary>
        /// 缺陷级别，记录系统发生不可弥补的错误异常信息。
        /// </summary>
        Fault = 5,
    }
}