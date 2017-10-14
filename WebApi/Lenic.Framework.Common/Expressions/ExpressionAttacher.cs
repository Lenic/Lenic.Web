using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Framework.Common.Expressions
{
    /// <summary>
    /// 表达式树查询附加器
    /// </summary>
    public class ExpressionAttacher
    {
        #region Public Constructors

        /// <summary>
        /// 初始化创建一个 <see cref="ExpressionAttacher"/> 类的实例对象。
        /// </summary>
        public ExpressionAttacher()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// 获取后设置检索数据源。
        /// </summary>
        public IQueryable DataSource { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 为数据源附加查询。
        /// </summary>
        /// <param name="originalElementType">附加查询的原始元素类型。</param>
        /// <param name="targetExpr">附加查询表达式树。</param>
        /// <returns>一个附加完成的查询分析器接口。</returns>
        public IQueryableParser Attach(Type originalElementType, Expression targetExpr)
        {
            var builder = new ExpresionRewriteBuilder(originalElementType, DataSource.ElementType);
            var parser = new QueryableParser { DataSource = DataSource, };
            parser.Converter = p =>
            {
                if (p is UnaryExpression)
                    p = (p as UnaryExpression).Operand;

                if (!(p is LambdaExpression))
                    throw new NotSupportedException();

                var lambda = p as LambdaExpression;
                var @parameters = lambda.Parameters.Where(r => r.Type == builder.SourceType).ToArray();

                if (!@parameters.Any())
                    return p;

                if (@parameters.Count() > 1)
                    throw new NotSupportedException();

                var parameterExpr = Expression.Parameter(DataSource.ElementType, @parameters[0].Name);

                var value = builder.Build(lambda.Body, expr =>
                {
                    if (!(expr is ParameterExpression))
                        return expr;

                    var pe = expr as ParameterExpression;
                    if (pe.Name == parameterExpr.Name && pe.Type == parameterExpr.Type)
                        return parameterExpr;
                    else
                        return expr;
                });
                var result = Expression.Lambda(value, parameterExpr);

                return result;
            };

            parser.Build(targetExpr as MethodCallExpression);

            return parser;
        }

        #endregion Public Methods
    }
}