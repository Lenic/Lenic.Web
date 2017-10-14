using System;
using Lenic.Framework.Common.Extensions;

namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 可扩展对象日志信息扩展方法集合
    /// </summary>
    public static class ObjectExtendibleExtensions
    {
        /// <summary>
        /// 标记对象为变更。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <param name="changeType">对象变更类型枚举。</param>
        /// <returns>修改后的对象实例。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[SetChange{{0}}].obj.With(typeof(T).FullName)</exception>
        public static ObjectExtendible<T> SetChange<T>(this ObjectExtendible<T> obj, ObjectChangeType changeType)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[SetChange<{0}>].obj".With(typeof(T).FullName));

            obj.Tag["Change"] = changeType.ToString();
            return obj;
        }

        /// <summary>
        /// 标记对象为变更。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <param name="changeType">文本形式表示的对象变更类型。</param>
        /// <returns>修改后的对象实例。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[SetChange{{0}}].obj.With(typeof(T).FullName)</exception>
        public static ObjectExtendible<T> SetChange<T>(this ObjectExtendible<T> obj, string changeType)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[SetChange<{0}>].obj".With(typeof(T).FullName));

            obj.Tag["Change"] = changeType;
            return obj;
        }

        /// <summary>
        /// 获取对象的变更文本描述信息，未获取到变更时返回空字符串。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <returns>对象的变更文本描述信息。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[SetChange{{0}}].obj.With(typeof(T).FullName)</exception>
        public static string GetChange<T>(this ObjectExtendible<T> obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[GetChange<{0}>].obj".With(typeof(T).FullName));

            return obj.Tag.GetValueX("Change", string.Empty) as string;
        }

        /// <summary>
        /// 设置对象的来源属性。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <param name="from">标记对象来源的字符串描述。</param>
        /// <returns>修改后的对象实例。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[SetFrom{{0}}].obj.With(typeof(T).FullName)</exception>
        public static ObjectExtendible<T> SetFrom<T>(this ObjectExtendible<T> obj, string from)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[SetFrom<{0}>].obj".With(typeof(T).FullName));

            obj.Tag["From"] = from;
            return obj;
        }

        /// <summary>
        /// 获取对象的来源文本描述信息，未获取到变更时返回空字符串。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <returns>对象的变更文本描述信息。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[GetFrom{{0}}].obj.With(typeof(T).FullName)</exception>
        public static string GetFrom<T>(this ObjectExtendible<T> obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[GetFrom<{0}>].obj".With(typeof(T).FullName));

            return obj.Tag.GetValueX("From", string.Empty) as string;
        }

        /// <summary>
        /// 获取对象的标题文本描述信息。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <returns>对象的变更文本描述信息。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[GetTitle{{0}}].obj.With(typeof(T).FullName)</exception>
        /// <exception cref="System.ArgumentException">没有发现设置的对象变更信息！</exception>
        public static string GetTitle<T>(this ObjectExtendible<T> obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[GetTitle<{0}>].obj".With(typeof(T).FullName));

            var change = GetChange(obj);
            if (string.IsNullOrWhiteSpace(change))
                throw new ArgumentException("没有发现设置的对象变更信息！");

            return "{0}.{1}".With(typeof(T).FullName, change);
        }

        /// <summary>
        /// 设置对象附加的一些参数信息。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <param name="value">对象参数的实例对象。</param>
        /// <returns>修改后的对象实例。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[SetParameter{{0}}].obj.With(typeof(T).FullName)</exception>
        public static ObjectExtendible<T> SetParameter<T>(this ObjectExtendible<T> obj, object value)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[SetParameter<{0}>].obj".With(typeof(T).FullName));

            obj.Tag["Parameter"] = value;
            return obj;
        }

        /// <summary>
        /// 获取对象的参数信息。
        /// </summary>
        /// <typeparam name="T">扩展的对象类型</typeparam>
        /// <param name="obj">一个可扩展的对象类的实例对象。</param>
        /// <returns>对象的参数信息。</returns>
        /// <exception cref="System.ArgumentNullException">[ObjectExtendibleExtensions].[GetParameter{{0}}].obj.With(typeof(T).FullName)</exception>
        public static T GetParameter<T>(this IObjectExtendible obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException("[ObjectExtendibleExtensions].[GetParameter<{0}>].obj".With(typeof(T).FullName));

            return (T)obj.Tag.GetValueX("Parameter", null);
        }
    }
}