using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class DefaultMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {
            return true;
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            return ParameterValueWriter.Write(GetValue(expression));
        }

        private static object GetValue(Expression input)
        {
            var objectMember = Expression.Convert(input, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

            return getterLambda();
        }
    }
}