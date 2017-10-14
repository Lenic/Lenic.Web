using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lenic.Framework.Common.Expressions
{
    /// <summary>
    /// Lambda 表达式树转换查询类
    /// </summary>
    public static class ConvertQueryable
    {
        /// <summary>
        /// 创建一个转换查询的数据源。
        /// </summary>
        /// <typeparam name="T">显式查询类型</typeparam>
        /// <param name="query">一个真实类型的查询接口获取方法。</param>
        /// <returns>一个显式查询的数据源。</returns>
        public static IQueryable<T> CreateQuery<T>(Func<IQueryable> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            return new ConvertQueryable<T> { DataSource = query };
        }

        /// <summary>
        /// 向原始查询附加额外的查询条件。
        /// </summary>
        /// <param name="source">一个原始查询的数据源。</param>
        /// <param name="query">需要附加的查询条件：ConvertQueryable 类型。</param>
        /// <returns>附加完毕查询的数据源。</returns>
        public static IQueryable Attach(this IQueryable source, IQueryable query)
        {
            if (!(query is IElementTypeProvider))
                throw new ArgumentException("The query is not ConvertQueryable<T> type.");

            var attacher = new ExpressionAttacher();
            attacher.DataSource = source;

            var parser = attacher.Attach((query as IElementTypeProvider).OriginalElementType, query.Expression);
            return parser.DataSource;
        }

        /// <summary>
        /// 向原始查询附加额外的查询条件。
        /// </summary>
        /// <typeparam name="TOriginalElementType">query 查询的原始元素类型</typeparam>
        /// <param name="source">一个原始查询的数据源。</param>
        /// <param name="query">需要附加的查询条件。</param>
        /// <returns>附加完毕查询的数据源。</returns>
        public static IQueryable Attach<TOriginalElementType>(this IQueryable source, IQueryable query)
        {
            var attacher = new ExpressionAttacher();
            attacher.DataSource = source;

            var parser = attacher.Attach(typeof(TOriginalElementType), query.Expression);
            return parser.DataSource;
        }

        /// <summary>
        /// 根据一个转换查询的数据源获取符合条件的第一个元素，没有符合的元素时将返回 <c>null</c> 。
        /// </summary>
        /// <param name="source">一个转换查询的数据源接口。</param>
        /// <returns>符合条件的第一个元素。</returns>
        public static object FirstOrDefault(IQueryable source)
        {
            return Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// 根据一个转换查询的数据源获取符合条件元素的数量。
        /// </summary>
        /// <param name="source">一个转换查询的数据源接口。</param>
        /// <returns>符合条件元素的数量。</returns>
        public static int Count(IQueryable source)
        {
            return (int)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// 根据一个转换查询的数据源获取符合条件元素的数量。
        /// </summary>
        /// <param name="source">一个转换查询的数据源接口。</param>
        /// <returns>符合条件元素的数量。</returns>
        public static long LongCount(IQueryable source)
        {
            return (long)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// 根据一个转换查询的数据源获取符合条件元素的是否存在：<c>true</c> 表示存在。
        /// </summary>
        /// <param name="source">一个转换查询的数据源接口。</param>
        /// <returns><c>true</c> 表示存在符合条件的元素；否则返回 <c>false</c> 。</returns>
        public static bool Any(IQueryable source)
        {
            return (bool)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// 根据一个转换查询的数据源获取符合条件的元素数组。
        /// </summary>
        /// <param name="source">一个转换查询的数据源接口。</param>
        /// <returns>符合条件的元素数组。</returns>
        public static object[] ToArray(IQueryable source)
        {
            return (Execute(source, MethodInfo.GetCurrentMethod()) as IEnumerable).OfType<object>().ToArray();
        }

        /// <summary>
        /// 转换 Lambda 表达式中的第一个参数为 {T} 类型。
        /// </summary>
        /// <typeparam name="T">需要转换的目标元素类型。</typeparam>
        /// <param name="lambda">需要转换的原始表达式树。</param>
        /// <returns>转换完成的目标表达式树。</returns>
        public static Expression<Func<T, bool>> ChangeParameter<T>(this LambdaExpression lambda)
        {
            return ChangeParameter(lambda, typeof(T)) as Expression<Func<T, bool>>;
        }

        /// <summary>
        /// 转换 Lambda 表达式中的第一个参数为 <param name="targetElementType"/> 类型。
        /// </summary>
        /// <param name="lambda">需要转换的原始表达式树。</param>
        /// <param name="targetElementType">需要转换的目标元素类型。</param>
        /// <returns>转换完成的目标表达式树。</returns>
        public static LambdaExpression ChangeParameter(this LambdaExpression lambda, Type targetElementType)
        {
            var originalParameter = lambda.Parameters[0];
            var parameterExpr = Expression.Parameter(targetElementType, originalParameter.Name);
            var builder = new ExpresionRewriteBuilder(originalParameter.Type, targetElementType);

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
        }

        private static object Execute(IQueryable source, MethodBase method)
        {
            return source.Provider.Execute(Expression.Call(null, (MethodInfo)method, new Expression[]
			{
				source.Expression
			}));
        }
    }

    internal class ConvertQueryable<T> : IOrderedQueryable<T>, IElementTypeProvider
    {
        public Func<IQueryable> DataSource { get; set; }

        public Type OriginalElementType { get; set; }

        public ConvertQueryable()
        {
            Expression = Expression.Constant(this);
            _provider = new Lazy<IQueryProvider>(() => new ConvertQueryProvider<T>
            {
                DataSource = DataSource,
                OriginalElementType = typeof(T),
            }, true);

            OriginalElementType = typeof(T);
        }

        public ConvertQueryable(Expression expression, Type originalElementType)
        {
            if (expression == null)
                throw new ArgumentNullException(string.Format("[RestQueryable<{0}>].[ctor].expression", typeof(T).Name), "查询表达式不能为 null 。");

            Expression = expression;
            OriginalElementType = originalElementType;

            _provider = new Lazy<IQueryProvider>(() => new ConvertQueryProvider<T>
            {
                DataSource = DataSource,
                OriginalElementType = originalElementType,
            }, true);
        }

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator()
        {
            var enumerable = Provider.Execute<IEnumerable<T>>(Expression);
            return (enumerable ?? new T[0]).GetEnumerator();
        }

        #endregion IEnumerable<T> 成员

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
        }

        #endregion IEnumerable 成员

        #region IQueryable 成员

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get { return _provider.Value; } }

        #endregion IQueryable 成员

        private Lazy<IQueryProvider> _provider = null;
    }
}