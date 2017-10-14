using Lenic.Framework.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Framework.Common.Expressions
{
    #region PredicateBuilder

    /// <summary>
    /// 谓词构建器
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 使用 Expression.AndAlso 的方式拼接两个 Expression 。
        /// </summary>
        /// <typeparam name="T">表达式中的元素类型</typeparam>
        /// <param name="left">左边的 Expression</param>
        /// <param name="right">右边的 Expression</param>
        /// <returns>拼接完成的 Expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return MakeBinary(left, right, Expression.AndAlso);
        }

        /// <summary>
        /// False子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        /// <summary>
        /// 拼接两个 Expression 。
        /// </summary>
        /// <typeparam name="T">表达式中的元素类型</typeparam>
        /// <param name="left">左边的 Expression</param>
        /// <param name="right">右边的 Expression</param>
        /// <param name="func">表达式拼接的具体逻辑</param>
        /// <returns>拼接完成的 Expression</returns>
        public static Expression<Func<T, bool>> MakeBinary<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> func)
        {
            var data = Combinate(right.Parameters, left.Parameters).ToArray();
            if (left.Parameters.Count != right.Parameters.Count || !data.All(p => p.Key.Type == p.Value.Type))
                throw new InvalidOperationException("Parameter error.");

            right = ParameterReplace.Replace(right, data) as Expression<Func<T, bool>>;
            return Expression.Lambda<Func<T, bool>>(func(left.Body, right.Body), left.Parameters);
        }

        /// <summary>
        /// 使用 Expression.Or 的方式拼接两个 Expression 。
        /// </summary>
        /// <typeparam name="T">表达式中的元素类型</typeparam>
        /// <param name="left">左边的 Expression</param>
        /// <param name="right">右边的 Expression</param>
        /// <returns>拼接完成的 Expression</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return MakeBinary(left, right, Expression.OrElse);
        }

        /// <summary>
        /// True子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        private static IEnumerable<KeyValuePair<T, T>> Combinate<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var a = left.GetEnumerator();
            var b = right.GetEnumerator();
            while (a.MoveNext() && b.MoveNext())
                yield return new KeyValuePair<T, T>(a.Current, b.Current);
        }
    }

    #endregion PredicateBuilder

    #region class: ParameterReplace

    /// <summary>
    /// Parameter Replace
    /// </summary>
    public class ParameterReplace : ExpressionVisitor
    {
        private Dictionary<ParameterExpression, ParameterExpression> parameters = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterReplace"/> class.
        /// </summary>
        /// <param name="paramList">The param list.</param>
        public ParameterReplace(IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            parameters = paramList.ToDictionary(p => p.Key, p => p.Value, new ParameterEquality());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterReplace"/> class.
        /// </summary>
        private ParameterReplace()
        {
        }

        /// <summary>
        /// Replaces the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="paramList">The param list.</param>
        /// <returns></returns>
        public static Expression Replace(Expression e, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            var item = new ParameterReplace(paramList);
            return item.Visit(e);
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression result;
            if (parameters.TryGetValue(p, out result))
                return result;
            else
                return base.VisitParameter(p);
        }

        #region class: ParameterEquality

        private class ParameterEquality : IEqualityComparer<ParameterExpression>
        {
            public bool Equals(ParameterExpression x, ParameterExpression y)
            {
                if (x == null || y == null)
                    return false;

                return x.Type == y.Type;
            }

            public int GetHashCode(ParameterExpression obj)
            {
                if (obj == null)
                    return 0;

                return obj.Type.GetHashCode();
            }
        }

        #endregion class: ParameterEquality
    }

    #endregion class: ParameterReplace
}