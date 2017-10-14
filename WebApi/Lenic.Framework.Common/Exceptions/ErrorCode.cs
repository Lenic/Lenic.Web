using System;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 错误代码类型枚举
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// 【缺省】未分类的错误代码。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 数据库类型的错误代码。
        /// </summary>
        Database = 1,

        /// <summary>
        /// 参数验证类型的错误代码。
        /// </summary>
        Argument = 2,

        /// <summary>
        /// 业务类型的错误代码。
        /// </summary>
        Business = 3,
    }
}