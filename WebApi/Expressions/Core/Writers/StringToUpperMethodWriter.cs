using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class StringToUpperMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(string)
                   && (expression.Method.Name == "ToUpper" || expression.Method.Name == "ToUpperInvariant");
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var obj = expression.Object;

            return string.Format("toupper({0})", expressionWriter(obj));
        }
    }
}