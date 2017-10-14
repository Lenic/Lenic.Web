using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lenic.Framework.Common.Expressions
{
    internal class ExpresionRewriteBuilder
    {
        public Type SourceType { get; set; }

        public Type TargetType { get; set; }

        public ExpresionRewriteBuilder(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }

        public Expression Build(Expression exp, Func<Expression, Expression> func)
        {
            Expression clone = null;
            var be = exp as BinaryExpression;
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    clone = Expression.AndAlso(Build(be.Left, func), Build(be.Right, func), be.Method);
                    break;

                case ExpressionType.OrElse:
                    clone = Expression.OrElse(Build(be.Left, func), Build(be.Right, func), be.Method);
                    break;

                case ExpressionType.Equal:
                    clone = Expression.Equal(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.GreaterThan:
                    clone = Expression.GreaterThan(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    clone = Expression.GreaterThanOrEqual(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.LessThan:
                    clone = Expression.LessThan(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.LessThanOrEqual:
                    clone = Expression.LessThanOrEqual(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.NotEqual:
                    clone = Expression.NotEqual(Build(be.Left, func), Build(be.Right, func), be.IsLiftedToNull, be.Method);
                    break;

                case ExpressionType.Not:
                    var ue = exp as UnaryExpression;
                    clone = Expression.Not(Build(ue.Operand, func));
                    break;

                case ExpressionType.MemberAccess:
                    var me = exp as MemberExpression;

                    MemberInfo newMember = me.Member;
                    Type newType = newMember.DeclaringType;
                    if (newType == SourceType)
                    {
                        newType = TargetType;
                        MemberInfo[] members = newType.GetMember(me.Member.Name);
                        if (members.Length == 1)
                        {
                            newMember = members[0];
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("{0}.{1}", newType.FullName, me.Member.Name));
                        }
                    }

                    var meParameter = Build(me.Expression, func);
                    if (meParameter.Type != newMember.DeclaringType)
                    {
                        MemberInfo[] members = meParameter.Type.GetMember(me.Member.Name);
                        if (members.Length == 1)
                        {
                            newMember = members[0];
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("{0}.{1}", meParameter.Type.FullName, me.Member.Name));
                        }
                    }
                    clone = Expression.MakeMemberAccess(meParameter, newMember);
                    break;

                case ExpressionType.Constant:
                    var ce = exp as ConstantExpression;
                    clone = Expression.Constant(ce.Value);
                    break;

                case ExpressionType.Parameter:
                    var pe = exp as ParameterExpression;
                    Type peNewType = pe.Type;
                    if (peNewType == SourceType)
                    {
                        peNewType = TargetType;
                    }
                    clone = Expression.Parameter(peNewType, pe.Name);
                    break;

                case ExpressionType.Call:
                    MethodCallExpression mce = exp as MethodCallExpression;
                    if (mce.Arguments != null && mce.Arguments.Count > 0)
                    {
                        List<Expression> expressionList = new List<Expression>();
                        foreach (Expression expression in mce.Arguments)
                        {
                            expressionList.Add(Build(expression, func));
                        }
                        clone = Expression.Call(Build(mce.Object, func), mce.Method, expressionList.ToArray());
                    }
                    else
                    {
                        clone = Expression.Call(Build(mce.Object, func), mce.Method);
                    }
                    break;

                case ExpressionType.Invoke:
                    InvocationExpression ie = exp as InvocationExpression;
                    List<Expression> arguments = new List<Expression>();
                    foreach (Expression expression in ie.Arguments)
                    {
                        arguments.Add(Build(expression, func));
                    }
                    clone = Build(ie.Expression, func);
                    //clone = Expression.Invoke(Rewrite(ie.Expression, c), arguments);
                    break;

                case ExpressionType.Convert:
                    var ue2 = exp as UnaryExpression;
                    //clone = Expression.Not(Rewrite(ue2.Operand, c));
                    clone = Expression.Convert(ue2.Operand, ue2.Type, ue2.Method);
                    break;

                case ExpressionType.New:
                    var newExpr = exp as NewExpression;
                    clone = Expression.New(newExpr.Constructor, newExpr.Arguments.Select(p => Build(p, func)).ToArray());
                    break;

                case ExpressionType.MemberInit:
                    var initObjectExpr = exp as MemberInitExpression;
                    var newExpr2 = Build(initObjectExpr.NewExpression, func) as NewExpression;
                    clone = Expression.MemberInit(newExpr2, initObjectExpr.Bindings.Select(p =>
                    {
                        MemberInfo newMember2 = null;
                        var members = newExpr2.Type.GetMember(p.Member.Name);
                        if (members.Length == 1)
                        {
                            newMember2 = members[0];
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("{0}.{1}", newExpr2.Type.FullName, p.Member.Name));
                        }
                        return Expression.Bind(newMember2, Build((p as MemberAssignment).Expression, func));
                    }));
                    break;

                case ExpressionType.NewArrayInit:
                    var arrayInitExpr = exp as NewArrayExpression;
                    clone = Expression.NewArrayInit(arrayInitExpr.Type.GetElementType(), arrayInitExpr.Expressions.Select(p => Build(p, func)));
                    break;

                case ExpressionType.ListInit:
                    var initListExpr = exp as ListInitExpression;
                    var newExpr3 = Build(initListExpr.NewExpression, func) as NewExpression;
                    clone = Expression.ListInit(newExpr3, initListExpr.Initializers.Select(p => Expression.ElementInit(p.AddMethod, p.Arguments.Select(r => Build(r, func)))));
                    break;

                default:
                    throw new NotImplementedException(exp.NodeType.ToString());
            }
            return func(clone);
        }
    }
}