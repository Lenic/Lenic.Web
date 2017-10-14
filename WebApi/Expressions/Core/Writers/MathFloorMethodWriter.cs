using System;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class MathFloorMethodWriter : MathMethodWriter
    {
        protected override string MethodName
        {
            get { return "floor"; }
        }

        public override bool CanHandle(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(Math)
                   && expression.Method.Name == "Floor";
        }
    }
}