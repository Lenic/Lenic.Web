using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 数据库异常类
    /// </summary>
    [Serializable]
    public sealed class DatabaseException : LevelException
    {
        /// <summary>
        /// 获取发生异常的数据库命令实例对象，<c>null</c> 时请参考 Message 属性.
        /// </summary>
        public DbCommand Command { get; private set; }

        /// <summary>
        /// 获取或设置异常发生时，一些额外的信息.
        /// </summary>
        public IDictionary<string, object> Tag { get; set; }

        /// <summary>
        /// 初始化新建一个 <see cref="DatabaseException" /> 类的实例对象.
        /// </summary>
        /// <param name="command">发生错误的数据库命令实例对象.</param>
        /// <param name="inner">ADO.NET 抛出的异常信息.</param>
        public DatabaseException(DbCommand command, Exception inner)
            : base(command.CommandText, inner)
        {
            Command = command;
            Tag = new Dictionary<string, object>();
            Level = ExceptionLevel.Error;
        }

        /// <summary>
        /// 初始化新建一个 <see cref="DatabaseException" /> 类的实例对象.
        /// </summary>
        /// <param name="message">发生错误的 SQL 语句.</param>
        /// <param name="inner">ADO.NET 抛出的异常信息.</param>
        public DatabaseException(string message, Exception inner)
            : base(message, inner)
        {
            Tag = new Dictionary<string, object>();
        }
    }
}