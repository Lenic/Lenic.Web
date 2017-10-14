using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// Expression extensions
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 获取目标类型的成员【名称/类型】信息列表.
        /// </summary>
        /// <param name="exp">用于获取类型的表达式树.</param>
        /// <returns>目标类型的成员【名称/类型】信息列表.</returns>
        /// <exception cref="System.ArgumentException">错误的表达式类型.</exception>
        public static IEnumerable<KeyValuePair<string, Type>> FetchMembers(this Expression exp)
        {
            if (exp == null)
                return Enumerable.Empty<KeyValuePair<string, Type>>();

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return FetchMembers(((LambdaExpression)exp).Body);
                case ExpressionType.New:
                    return (exp as NewExpression).Members.Select(p => new KeyValuePair<string, Type>(GetName(p.Name), GetRetutnType(p)));
                case ExpressionType.MemberInit:
                    return (exp as MemberInitExpression).Bindings.Select(p => new KeyValuePair<string, Type>(GetName(p.Member.Name), GetRetutnType(p.Member)));
                case ExpressionType.Convert:
                    return FetchMembers((exp as UnaryExpression).Operand);
                case ExpressionType.MemberAccess:
                    var member = exp as MemberExpression;
                    var item = new KeyValuePair<string, Type>(GetName(member.Member.Name), GetRetutnType(member.Member));
                    return new KeyValuePair<string, Type>[] { item };
                case ExpressionType.Parameter:
                    return (exp as ParameterExpression).Type.GetProperties().Select(p => new KeyValuePair<string, Type>(GetName(p.Name), p.PropertyType));
                default:
                    throw new ArgumentException("错误的表达式类型.");
            }
        }

        /// <summary>
        /// 获取目标类型的成员【名称/类型】信息列表.
        /// </summary>
        /// <typeparam name="T">待获取信息的类型.</typeparam>
        /// <param name="expr">用于获取类型的表达式树.</param>
        /// <returns>目标类型的成员【名称/类型】信息列表.</returns>
        public static IEnumerable<KeyValuePair<string, Type>> GetMembers<T>(this Expression<Func<T, object>> expr)
        {
            return FetchMembers(expr);
        }

        private static Type GetRetutnType(MemberInfo member)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).PropertyType;
            else if (member is MethodInfo)
                return (member as MethodInfo).ReturnType;
            else
                throw new ArgumentException("member");
        }

        private static string GetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (name.Contains('_'))
            {
                var index = name.IndexOf('_');
                return name.Substring(index + 1);
            }
            else
                return name;
        }
    }
}