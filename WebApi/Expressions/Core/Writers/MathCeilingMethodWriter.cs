using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class MathCeilingMethodWriter : MathMethodWriter
    {
        protected override string MethodName
        {
            get { return "ceiling"; }
        }

        public override bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(Math)
                   && expression.Method.Name == "Ceiling";
        }
    }
}