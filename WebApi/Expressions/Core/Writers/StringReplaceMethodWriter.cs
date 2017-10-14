using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class StringReplaceMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                   && expression.Method.Name == "Replace";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var firstArgument = expression.Arguments[0];
            var secondArgument = expression.Arguments[1];
            var obj = expression.Object;

            return string.Format(
                "replace({0}, {1}, {2})",
                expressionWriter(obj),
                expressionWriter(firstArgument),
                expressionWriter(secondArgument));
        }
    }
}