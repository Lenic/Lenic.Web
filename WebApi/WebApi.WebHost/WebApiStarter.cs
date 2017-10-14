using Lenic.Framework.Caching;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Messaging;
using Lenic.Web.WebApi.ExtensionPoints;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Lenic.Web.WebApi.WebHost.UnityConfiguration;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// WebApi 配置启动入口
    /// </summary>
    public static class WebApiStarter
    {
        #region Business Methods

        /// <summary>
        /// 向当前网站中注册配置信息。
        /// </summary>
        public static void Register()
        {
            // 初始化 IOC 配置
            var container = BuildUnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            // 加载缺省配置
            ConfigurationPipeline.ContainerConfiguration.Add(new AssemblyResolveConfiguration());
            ConfigurationPipeline.ContainerConfiguration.Add(new CustomConfigConfiguration());
            ConfigurationPipeline.ContainerConfiguration.Add(new UnityContainerConfiguration());

            ConfigurationPipeline.HttpConfiguration.Add(new AppConfigConfiguration());
            ConfigurationPipeline.HttpConfiguration.Add(new DefaultHttpRouteConfiguration());
            ConfigurationPipeline.HttpConfiguration.Add(new RequestConfiguration());
            ConfigurationPipeline.HttpConfiguration.Add(ConfigurationCreator.CreateHttpConfigurationFromDelegate(p =>
            {
                p.Filters.Add(new ReturnValueWrapperAttribute());
                p.Filters.Add(new ReturnExceptionWrapperAttribute());
            }));

            // 触发自定义初始化加载
            GlobalStarterX.Instance.RaiseEvent(p => p.Initialize());

            // 加载 IOC 配置
            ConfigurationPipeline.ContainerConfiguration.Aggregate((object)container, (x, y) => y.ConfigurateContainer(x));
            // 加载 其它 配置
            ConfigurationPipeline.HttpConfiguration.ForEach(p => p.Configurate(GlobalConfiguration.Configuration));

            // 触发配置完成事件
            GlobalStarterX.Instance.RaiseEvent(p => p.Complete());
        }

        #endregion Business Methods

        #region Private Methods

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterInstance(GlobalConfiguration.Configuration)
                     .RegisterInstance<IMessageRouter>(new MessageRouter())
                     .RegisterInstance<ICacheContainer>(new HttpCacheContainer())
                     .RegisterType<IHttpControllerSelector, ModuleHttpControllerSelector>()
                     .RegisterType<IndexableHandler, DataContextHandler>("DataContext")
                     .RegisterType<IndexableHandler, RequestHeaderInfomationFetchHandler>("RequestHeaderInfomationFetch");

            var adapter = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => adapter);

            return container;
        }

        #endregion Private Methods
    }
}