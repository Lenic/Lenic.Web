using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal interface IMethodCallWriter
    {
        bool CanHandle(MethodCallExpression expression);

        string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter);
    }
}