using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core
{
    public class RestQueryProvider<T> : IQueryProvider<T>
    {
        #region IQueryProvider 成员

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new RestQueryable<TElement>(DataFetcher, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new RestQueryable<T>(DataFetcher, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            var dataParameter = new RemoteDataParameter();

            var methodCallExpression = expression as MethodCallExpression;
            dataParameter.Executor = methodCallExpression.Method.Name;

            var processor = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionProcessor>();
            processor.DataParameter = dataParameter;
            processor.Writer = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionWriter>();

            processor.Build(methodCallExpression);

            return DataFetcher.GetObject(dataParameter, methodCallExpression.Method.ReturnType);
        }

        #endregion IQueryProvider 成员

        #region IQueryProvider<T> 成员

        public IRemoteDataFetcher DataFetcher { get; set; }

        #endregion IQueryProvider<T> 成员
    }
}