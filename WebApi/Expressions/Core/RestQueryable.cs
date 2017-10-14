using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core
{
    public class RestQueryable<T> : IOrderedQueryable<T>
    {
        public IRemoteDataFetcher DataFetcher { get; set; }

        public RestQueryable(IRemoteDataFetcher dataFetcher)
        {
            DataFetcher = dataFetcher;
            Expression = Expression.Constant(this);

            var provider = RemoteObjectContext.DefaultObjectResolver.GetInstance<IQueryProvider<T>>();
            provider.DataFetcher = DataFetcher;
            Provider = provider;
        }

        public RestQueryable(IRemoteDataFetcher dataFetcher, Expression expression)
            : this(dataFetcher)
        {
            if (expression == null)
                throw new ArgumentNullException(string.Format("[RestQueryable<{0}>].[ctor].expression", typeof(T).Name), "查询表达式不能为 null 。");

            Expression = expression;
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

        public IQueryProvider Provider { get; private set; }

        #endregion IQueryable 成员
    }
}