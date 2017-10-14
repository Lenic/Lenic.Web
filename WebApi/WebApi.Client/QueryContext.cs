using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// 客户端查询上下文：需要和 IHttpProxy 对象一起使用
    /// </summary>
    public static class QueryContext
    {
        /// <summary>
        /// 创建一个查询条件：该查询不能用调用终结函数，否则发生异常。
        /// </summary>
        /// <typeparam name="T">查询的元素类型。</typeparam>
        /// <returns>一个 <see cref="IQueryable{T}"/> 类型的实例对象。</returns>
        public static IQueryable<T> CreateQuery<T>()
        {
            return new VirtualQueryable<T>();
        }

        /// <summary>
        /// 获取查询条件的查询表达式树。
        /// </summary>
        /// <typeparam name="T">查询的元素类型。</typeparam>
        /// <param name="source">一个 <see cref="IQueryable{T}"/> 类型的实例对象。</param>
        /// <returns>获取得到的查询表达式树。</returns>
        public static LambdaExpression Query<T>(this IQueryable<T> source)
        {
            return Expression.Lambda(source.Expression);
        }

        #region Private Class：VirtualQueryable<T>

        private class VirtualQueryable<T> : IOrderedQueryable<T>
        {
            public VirtualQueryable()
            {
                Expression = Expression.Constant(this);
                Provider = new VirtualQueryProvider<T>(this);
            }

            #region IEnumerable<T> 成员

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion IEnumerable<T> 成员

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion IEnumerable 成员

            #region IQueryable 成员

            public Type ElementType
            {
                get { return typeof(T); }
            }

            public Expression Expression { get; internal set; }

            public IQueryProvider Provider { get; private set; }

            #endregion IQueryable 成员
        }

        #endregion Private Class：VirtualQueryable<T>

        #region Private Class：VirtualQueryProvider<T>

        private class VirtualQueryProvider<T> : IQueryProvider
        {
            private VirtualQueryable<T> _query;

            public VirtualQueryProvider(VirtualQueryable<T> query)
            {
                _query = query;
            }

            #region IQueryProvider 成员

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                _query.Expression = expression;

                return (IQueryable<TElement>)_query;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                throw new NotImplementedException();
            }

            public TResult Execute<TResult>(Expression expression)
            {
                throw new NotImplementedException();
            }

            public object Execute(Expression expression)
            {
                throw new NotImplementedException();
            }

            #endregion IQueryProvider 成员
        }

        #endregion Private Class：VirtualQueryProvider<T>
    }
}