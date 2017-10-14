using System;
using System.Collections.Generic;
using System.Linq;

namespace Lenic.Web.WebApi.Services
{
    /// <summary>
    /// 全局实例对象处理器参数列表
    /// </summary>
    public class GlobalObjectParameters
    {
        /// <summary>
        /// 获取或设置参数列表数据源。
        /// </summary>
        public IDictionary<string, object> DataSource { get; set; }

        /// <summary>
        /// 初始化创建一个 <see cref="GlobalObjectParameters"/> 类的实例对象。
        /// </summary>
        public GlobalObjectParameters()
        {
        }
    }

    public static class GlobalObjectParametersExtensions
    {
        #region Private Fields

        private static readonly Func<string> DefaultStringFunc = () => string.Empty;
        private static readonly Func<IDictionary<string, object>> DefaultDictionaryFunc = () => new Dictionary<string, object>();
        private static readonly Func<Uri> DefaultUriFunc = () => null;
        private static readonly Func<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> DefaultNameValuesFunc = () => Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();

        #endregion Private Fields

        #region Business Methods

        /// <summary>
        /// 获取当前执行的 Action 名称，未找到时返回空字符串。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action 名称。</returns>
        public static string GetActionName(this GlobalObjectParameters obj)
        {
            return obj.GetValue<string>("ActionName", DefaultStringFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action 参数列表，未找到时返回一个空的参数列表。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action 参数列表。</returns>
        public static IDictionary<string, object> GetActionArguments(this GlobalObjectParameters obj)
        {
            return obj.GetValue<IDictionary<string, object>>("ActionArguments", DefaultDictionaryFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action 的原始请求 Uri，未找到时返回 <c>null</c> 。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action 的原始请求 Uri。</returns>
        public static Uri GetRequestUri(this GlobalObjectParameters obj)
        {
            return obj.GetValue<Uri>("RequestUri", DefaultUriFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action 路由数据列表，未找到时返回一个空的路由数据列表。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action 路由数据列表。</returns>
        public static IDictionary<string, object> GetRouteValues(this GlobalObjectParameters obj)
        {
            return obj.GetValue<IDictionary<string, object>>("RouteValues", DefaultDictionaryFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action Http 请求方法，未找到时返回一个空字符串。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action Http 请求方法。</returns>
        public static string GetRequestMethod(this GlobalObjectParameters obj)
        {
            return obj.GetValue<string>("RequestMethod", DefaultStringFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action Http 请求 Header 集合，未找到时返回一个空集合。
        /// </summary>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <returns>当前执行的 Action Http 请求 Header 集合。</returns>
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetRequestHeaders(this GlobalObjectParameters obj)
        {
            return obj.GetValue<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>("RequestHeaders", DefaultNameValuesFunc);
        }

        /// <summary>
        /// 获取当前执行的 Action 参数的值，未找到时从
        /// <paramref name="defaultValueFunc"/>委托中获取值。
        /// </summary>
        /// <typeparam name="T">参数值的具体类型</typeparam>
        /// <param name="obj">一个 <see cref="GlobalObjectParameters"/> 类的实例对象。</param>
        /// <param name="key">Action 参数的名称。</param>
        /// <param name="defaultValueFunc">从当前参数集合中未找到值时，返回缺省值用到的委托方法。</param>
        /// <returns>当前执行的 Action 参数的值。</returns>
        public static T GetValue<T>(this GlobalObjectParameters obj, string key, Func<T> defaultValueFunc)
        {
            object result;
            if (obj.DataSource.TryGetValue(key, out result))
            {
                try
                {
                    return (T)result;
                }

                catch
                {
                    return defaultValueFunc();
                }
            }

            else
                return defaultValueFunc();
        }

        #endregion Business Methods
    }
}