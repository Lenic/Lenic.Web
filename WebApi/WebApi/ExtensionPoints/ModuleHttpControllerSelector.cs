using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;

namespace Lenic.Web.WebApi.ExtensionPoints
{
    /// <summary>
    /// 包含 Module 信息的 Http 请求控制器选择器类
    /// </summary>
    public class ModuleHttpControllerSelector : IHttpControllerSelector
    {
        #region Constant Variable

        private const string ModuleKey = "module";
        private const string NamespaceKey = "namespace";

        #endregion Constant Variable

        #region Private Fields

        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;
        private readonly HashSet<string> _duplicates;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="ModuleHttpControllerSelector"/> 类的实例对象。
        /// </summary>
        public ModuleHttpControllerSelector()
        {
            _configuration = Toolkit.GetInstance<HttpConfiguration>();
            _duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
        }

        #endregion Entrance

        #region IHttpControllerSelector 成员

        /// <summary>
        /// 根据一个 <see cref="System.Net.Http.HttpRequestMessage"/> 类的实例对象获取关联的 Http 请求处理类的描述对象。
        /// </summary>
        /// <param name="request">一个被包装过的 Http 请求实例对象。</param>
        /// <returns>关联的 Http 请求处理类的描述对象。</returns>
        /// <exception cref="System.Web.Http.HttpResponseException">NotFound（404）</exception>
        /// <exception cref="System.Web.Http.HttpResponseException">有多个控制器符合这个请求！</exception>
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var moduleName = routeData.GetRouteVariable(ModuleKey);

            string namespaceName = routeData.GetRouteVariable(NamespaceKey);
            if (namespaceName == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            string controllerName = routeData.GetRouteVariable(DefaultHttpControllerSelector.ControllerSuffix);
            if (controllerName == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            string key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", namespaceName, controllerName);
            if (!string.IsNullOrWhiteSpace(moduleName))
                key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", moduleName, key);

            HttpControllerDescriptor controllerDescriptor;
            if (_controllers.Value.TryGetValue(key, out controllerDescriptor))
                return controllerDescriptor;

            if (_duplicates.Contains(key))
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.InternalServerError, "有多个控制器符合这个请求！"));
            else
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// 获取缓存的控制器映射实例对象。
        /// </summary>
        /// <returns>缓存的控制器映射实例对象。</returns>
        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers.Value;
        }

        #endregion IHttpControllerSelector 成员

        #region Private Methods

        private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

            _configuration.Services.GetHttpControllerTypeResolver()
                .GetControllerTypes(_configuration.Services.GetAssembliesResolver())
                .ForEach(type =>
                {
                    var moduleName = string.Empty;
                    var module = DynamicModules.Instance.Modules.FirstOrDefault(p => p.LoadedAssemblies.Contains(type.Assembly));
                    if (module != null)
                        moduleName = module.Configuration.Name;

                    var segments = type.Namespace.Split(Type.Delimiter);
                    var controllerName = type.Name.Remove(type.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);

                    var key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", segments[segments.Length - 1], controllerName);
                    if (!string.IsNullOrWhiteSpace(moduleName))
                        key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", moduleName, key);

                    if (dictionary.Keys.Contains(key))
                        _duplicates.Add(key);
                    else
                        dictionary[key] = new HttpControllerDescriptor(_configuration, type.Name, type);
                });
            _duplicates.ForEach(p => dictionary.Remove(p));

            return dictionary;
        }

        #endregion Private Methods
    }
}