using Lenic.Web.WebApi.Services;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Microsoft.Practices.Unity;
using System;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 授权模块入口点
    /// </summary>
    public class EntryPoint : GlobalStarter
    {
        /// <summary>
        /// 引发当前程序集的初始化操作：该操作在初始化 IOC 基本配置，加载默认的配置管道后触发，触发索引【1】。
        /// </summary>
        public override void Initialize()
        {
            ConfigurationPipeline.ContainerConfiguration.Add(ConfigurationCreator.CreateContainerConfigurationFromDelegate(container =>
            {
                var unity = container as IUnityContainer;

                var validator = new DefaultAuthenticationTokenValidator()
                {
                    SlideDue = TimeSpan.FromHours(2),
                };

                unity.RegisterType<IIdentity, DefaultIdentity>("Default")
                     .RegisterType<IIdentity, UserTokenController>("UserToken")
                     .RegisterInstance<IAuthenticationTokenRegister>(validator)
                     .RegisterInstance<IAuthenticationTokenValidator>(validator);

                return container;
            }));
            ConfigurationPipeline.HttpConfiguration.Add(ConfigurationCreator.CreateHttpConfigurationFromDelegate(configuration => configuration.Filters.Add(new PrincipalAuthorizationFilter())));
        }
    }
}