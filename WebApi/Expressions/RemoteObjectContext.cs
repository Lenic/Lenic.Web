using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lenic.Web.WebApi.Expressions.Core;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 远程对象上下文类
    /// </summary>
    public static class RemoteObjectContext
    {
        /// <summary>
        /// 获取或设置缺省的服务对象获取器。
        /// </summary>
        public static IObjectResolver DefaultObjectResolver { get; set; }

        /// <summary>
        /// 初始化 <see cref="RemoteObjectContext" /> 类。
        /// </summary>
        static RemoteObjectContext()
        {
            var resolver = new DefaultObjectResolver();
            resolver.RegisterGenericTypeDefinition(typeof(IQueryProvider<>), typeof(RestQueryProvider<>));
            resolver.Register<IExpressionProcessor>(p => new ExpressionProcessor());
            resolver.Register<IExpressionWriter>(p => new ExpressionWriter());

            DefaultObjectResolver = resolver;
        }

        /// <summary>
        /// 获取一个 <see cref="IQueryable{TSource}" /> 类型的 RESTful 查询。
        /// </summary>
        /// <typeparam name="TSource">当前查询的元素类型。</typeparam>
        /// <param name="dataFetcher">一个远程数据获取器的实例对象。</param>
        /// <returns>一个可以链式编写的 <see cref="IQueryable{TSource}" /> 查询。</returns>
        public static IQueryable<TSource> AsQuerable<TSource>(this IRemoteDataFetcher dataFetcher)
        {
            return new RestQueryable<TSource>(dataFetcher);
        }

        /// <summary>
        /// 获取扩展的对象信息。
        /// </summary>
        /// <typeparam name="TSource">当前查询的元素类型。</typeparam>
        /// <param name="source">包含要转换的元素的 <see cref="System.Linq.IQueryable" />。</param>
        /// <param name="paths">要扩展的对象路径格式，比如：Child1, Child2/GrandChild2</param>
        /// <returns>一个可以链式编写的 <see cref="IQueryable{TSource}" /> 查询。</returns>
        public static IQueryable<TSource> Expand<TSource>(this IQueryable<TSource> source, string paths)
        {
            if (!(source is RestQueryable<TSource>))
                return source;

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new[] { typeof(TSource) });
            var methodCallExpression = Expression.Call(null, methodInfo, new[] { source.Expression, Expression.Constant(paths) });

            return source.Provider.CreateQuery<TSource>(methodCallExpression);
        }
    }
}