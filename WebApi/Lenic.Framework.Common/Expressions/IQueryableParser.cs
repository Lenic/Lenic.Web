using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Framework.Common.Expressions
{
    /// <summary>
    /// 查询分析器接口
    /// </summary>
    public interface IQueryableParser
    {
        #region Public Properties

        /// <summary>
        /// 获取查询数据源。
        /// </summary>
        IQueryable DataSource { get; }

        /// <summary>
        /// 获取最终执行的方法名称：<c>null</c> 表示没有执行数据查询。
        /// </summary>
        string FinalMethodName { get; }

        /// <summary>
        /// 获取最终执行数据查询的结果：如果无执行方法，该值为 <c>null</c> 。
        /// </summary>
        object Value { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 分析并执行数据源的查询操作：根据表达式树，如不包含最终执行方法，只会为数据源附加查询条件。
        /// </summary>
        /// <param name="methodCall"></param>
        void Build(MethodCallExpression methodCall);

        #endregion Public Methods
    }
}