using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal abstract class MathMethodWriter : IMethodCallWriter
    {
        protected abstract string MethodName { get; }

        public abstract bool CanHandle(MethodCallExpression expression);

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var mathArgument = expression.Arguments[0];

            return string.Format("{0}({1})", MethodName, expressionWriter(mathArgument));
        }
    }
}