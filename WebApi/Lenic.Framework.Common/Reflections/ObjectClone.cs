using System;
using System.Linq;

namespace Lenic.Framework.Common.Reflections
{
    /// <summary>
    /// 对象克隆类
    /// </summary>
    /// <typeparam name="T">待克隆的类型</typeparam>
    public static class ObjectClone<T>
    {
        private static TypeX type = typeof(T);

        /// <summary>
        /// 拷贝对象所有字段值到一个 ojbect 类型的数组中, 在对象中所有字段均为 CLR 和结构型时, 等同于 Clone 函数, 但效率高效.
        /// </summary>
        /// <param name="obj">待拷贝的对象.</param>
        /// <returns>对象字段值数组.</returns>
        public static object[] CopyValues(T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            return type.FullFields.Select(p => p.GetValue(obj)).ToArray();
        }

        /// <summary>
        /// 根据 source 对象更新 target 对象, 使得两个对象相同.
        /// </summary>
        /// <param name="target">源对象.</param>
        /// <param name="source">目标对象.</param>
        public static void Update(T target, T source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            foreach (var item in type.FullFields)
            {
                var field = item.GetValue(source);
                item.SetValue(target, field);
            }
        }

        /// <summary>
        /// 根据对象字段值数组更新一个实例对象.
        /// </summary>
        /// <param name="target">待更新的对象.</param>
        /// <param name="data">对象字段值数组.</param>
        public static void Update(T target, object[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (target == null) throw new ArgumentNullException("target");
            var count = type.FullFields.Count();
            if (count != data.Length)
                throw new ArgumentException(string.Format("the count of data isn't same to {0}'s count.", type.Name));

            int index = 0;
            foreach (var item in type.FullFields)
            {
                item.SetValue(target, data[index]);
                index++;
            }
        }
    }
}