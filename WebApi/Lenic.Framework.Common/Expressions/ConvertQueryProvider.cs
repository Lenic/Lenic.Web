using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Framework.Common.Expressions
{
    internal class ConvertQueryProvider<T> : IQueryProvider, IElementTypeProvider
    {
        public Func<IQueryable> DataSource { get; set; }

        public Type OriginalElementType { get; set; }

        #region IQueryProvider 成员

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new ConvertQueryable<TElement>(expression, OriginalElementType)
            {
                DataSource = DataSource
            };
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return new ConvertQueryable<T>(expression, OriginalElementType)
            {
                DataSource = DataSource
            };
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            var attacher = new ExpressionAttacher
            {
                DataSource = DataSource()
            };

            var parser = attacher.Attach(OriginalElementType, expression);

            return parser.Value;
        }

        #endregion IQueryProvider 成员
    }
}