using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class StringToLowerMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                   && (expression.Method.Name == "ToLower" || expression.Method.Name == "ToLowerInvariant");
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var obj = expression.Object;

            return string.Format("tolower({0})", expressionWriter(obj));
        }
    }
}