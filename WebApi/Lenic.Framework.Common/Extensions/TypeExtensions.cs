using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// Type 类型扩展信息集合
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// * 获取所有基类（不包括object）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetAllBaseType(this Type type)
        {
            List<Type> res = new List<Type>();

            res.Add(type);

            if (type.BaseType != typeof(object))
            {
                res.AddRange(type.BaseType.GetAllBaseType());
            }

            return res;
        }
    }
}