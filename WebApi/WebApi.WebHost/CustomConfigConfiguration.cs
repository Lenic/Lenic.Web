using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Microsoft.Practices.Unity;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 动态运行模块自定义配置配置类
    /// </summary>
    public class CustomConfigConfiguration : IContainerConfiguration
    {
        #region IContainerConfiguration 成员

        /// <summary>
        /// 配置 IOC/DI 容器。
        /// </summary>
        /// <param name="container">用于向其配置的 IOC/DI 容器实例对象：为空时需要创建一个并返回。</param>
        /// <returns>配置完成的 IOC/DI 容器实例对象。</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConfigurateContainer(object container)
        {
            var unity = container as IUnityContainer;

            unity.RegisterType<IAppConfigHandler, ConnectionSettingsConfigHandler>("ConnectionSettings");

            return container;
        }

        #endregion IContainerConfiguration 成员
    }
}