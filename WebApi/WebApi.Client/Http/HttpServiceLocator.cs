using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Lenic.Web.WebApi.Client.Http
{
    /// <summary>
    /// 尝试获取实例对象。
    /// </summary>
    /// <param name="serviceType">要获取的实例对象类型。</param>
    /// <param name="name">目标实例对象在容器中保存的键。</param>
    /// <param name="value">获取得到的目标实例对象：未获取到时返回 <c>null</c>。</param>
    /// <returns><c>true</c> 表示成功获取；否则返回 <c>false</c>。</returns>
    public delegate bool TryGetInstance(Type serviceType, string name, out object value, out Exception e);

    /// <summary>
    /// 向容器中注册特定的实例对象。
    /// </summary>
    /// <param name="serviceType">实例对象的显式类型。</param>
    /// <param name="name">实例对象在容器中保存的键。</param>
    /// <param name="value">实例对象的值。</param>
    public delegate void RegisterInstance(Type serviceType, string name, object value);

    /// <summary>
    /// IOC 服务容器适配器 Http 多重实现
    /// </summary>
    public sealed class HttpServiceLocator : ServiceLocatorImplBase
    {
        /// <summary>
        /// IOC 服务容器适配器的缺省对象实例。
        /// </summary>
        public static readonly HttpServiceLocator DefaultInstance = new HttpServiceLocator();

        /// <summary>
        /// 用于判断是否需要构建 Http 客户端代理的命名空间配置。
        /// </summary>
        public static string[] Namespaces { get; set; }

        /// <summary>
        /// 获取或设置根据命名空间前缀，获取 Http 客户端实例对象的委托。
        /// </summary>
        public Func<string, HttpClient> GetHttpClient { get; set; }

        /// <summary>
        /// 尝试从 IOC 容器中获取目标类型的实例对象的委托。
        /// </summary>
        public TryGetInstance TryGetValue { get; set; }

        /// <summary>
        /// 根据类型信息从 IOC 容器中获取全部注册实例的委托。
        /// </summary>
        public Func<Type, object[]> GetValues { get; set; }

        /// <summary>
        /// 构建完成 Http 客户端代理后，需要将代理对象加入 IOC 的委托。
        /// </summary>
        public RegisterInstance RegisterValue { get; set; }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return GetValues(serviceType);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            object value = null;
            Exception e = null;
            if (TryGetValue(serviceType, key, out value, out e))
                return value;

            var item = Namespaces.FirstOrDefault(p => serviceType.Namespace.StartsWith(p));
            if (item != null)
            {
                var proxy = (IHttpProxy)Activator.CreateInstance(typeof(HttpRealProxy<>).MakeGenericType(serviceType));
                proxy.ClientProxy = GetHttpClient(item);

                var obj = proxy.BuildProxyInstance();
                RegisterValue(serviceType, key, obj);

                return obj;
            }

            throw e;
        }
    }
}