using System;
using System.ComponentModel;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// object extension
    /// </summary>
    public static class ObjectExtension
    {
        #region Type Convert Extensions

        private static string typeIConvertibleFullName = typeof(IConvertible).FullName;

        /// <summary>
        /// 将当前实例对象类型转换为 T 类型.
        /// </summary>
        /// <typeparam name="T">目标类型.</typeparam>
        /// <param name="obj">当前实例.</param>
        /// <returns>
        /// 转换完成的 T 类型的一个实例对象.
        /// </returns>
        public static T ChangeType<T>(this object obj)
        {
            return ChangeType(obj, default(T));
        }

        /// <summary>
        /// 将当前实例对象类型转换为 T 类型.
        /// </summary>
        /// <typeparam name="T">目标类型.</typeparam>
        /// <param name="obj">当前实例.</param>
        /// <param name="defaultValue">转换失败时的返回值.</param>
        /// <returns>
        /// 转换完成的 T 类型的一个实例对象.
        /// </returns>
        /// <exception cref="System.ApplicationException">convert error.</exception>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static T ChangeType<T>(this object obj, T defaultValue)
        {
            if (obj != null && !(obj is DBNull))
            {
                if (obj is T)
                    return (T)obj;

                var sourceType = obj.GetType();
                var targetType = typeof(T);

                if (targetType.IsEnum)
                    return (T)Enum.Parse(targetType, obj.ToString(), true);

                if (sourceType.GetInterface(typeIConvertibleFullName) != null &&
                    targetType.GetInterface(typeIConvertibleFullName) != null)
                    return (T)Convert.ChangeType(obj, targetType);

                var converter = TypeDescriptor.GetConverter(obj);
                if (converter != null && converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(obj, targetType);

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(sourceType))
                    return (T)converter.ConvertFrom(obj);

                throw new ApplicationException("convert error.");
            }
            throw new ArgumentNullException("obj");
        }

        /// <summary>
        /// 将当前实例对象类型转换为 T 类型.
        /// </summary>
        /// <typeparam name="T">目标类型.</typeparam>
        /// <param name="obj">当前实例.</param>
        /// <param name="defaultValue">转换失败时的返回值.</param>
        /// <param name="ignoreException">如果设置为 <c>true</c> 表示忽略异常信息, 直接返回缺省值.</param>
        /// <returns>
        /// 转换完成的 T 类型的一个实例对象.
        /// </returns>
        public static T ChangeType<T>(this object obj, T defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return ChangeType<T>(obj, defaultValue);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return ChangeType<T>(obj, defaultValue);
        }

        #endregion Type Convert Extensions

        /// <summary>
        /// 设置当前对象后，再将其返回：不可对实例本身进行赋值操作；当前实例为空时跳过 action 操作。
        /// </summary>
        /// <typeparam name="T">待设置对象的类型。</typeparam>
        /// <param name="obj">当前带设置的实例对象。</param>
        /// <param name="action">需要执行的设置操作。</param>
        /// <returns>设置完成后的实例对象。</returns>
        public static T Setup<T>(this T obj, Action<T> action) where T : class
        {
            if (object.ReferenceEquals(obj, null))
                return obj;

            action(obj);
            return obj;
        }
    }
}