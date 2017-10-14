using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class StringStartsWithMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                   && expression.Method.Name == "StartsWith";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var argumentExpression = expression.Arguments[0];
            var obj = expression.Object;

            return string.Format(
                "startswith({0}, {1})",
                expressionWriter(obj),
                expressionWriter(argumentExpression));
        }
    }
}