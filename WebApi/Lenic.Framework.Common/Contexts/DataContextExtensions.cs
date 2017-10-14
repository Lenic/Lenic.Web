using System;

namespace Lenic.Framework.Common.Contexts
{
    /// <summary>
    /// 共享数据上下文扩展方法集合
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// 根据指定的键获取保存在上下文中的数据，如果该值不存在则通过 valueFactory 获取新值，同时添加到上下文数据中.
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="obj">一个共享数据上下文的实例对象</param>
        /// <param name="key">用于获取目标数据的键</param>
        /// <param name="valueFactory">用于创建新值的工厂方法</param>
        /// <returns>成功获取到的新值</returns>
        public static T GetValueWithFactory<T>(this DataContext obj, string key, Func<T> valueFactory) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var value = obj.GetValue<T>(key, null);
            if (value == null)
            {
                value = valueFactory();
                obj.SetValue(key, value);
            }
            return value;
        }
    }
}