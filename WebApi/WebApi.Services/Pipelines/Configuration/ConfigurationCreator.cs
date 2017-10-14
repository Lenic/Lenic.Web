using System;
using System.Web.Http;

namespace Lenic.Web.WebApi.Services.Pipelines.Configuration
{
    /// <summary>
    /// 配置管道配置节创建者
    /// </summary>
    public static class ConfigurationCreator
    {
        #region Public Methods

        /// <summary>
        /// 创建一个 IOC 容器的配置节。
        /// </summary>
        /// <param name="action">IOC 配置节逻辑。</param>
        /// <returns>配置节的实例对象。</returns>
        public static IContainerConfiguration CreateContainerConfigurationFromDelegate(Func<object, object> action)
        {
            return new DefaultContainerConfiguration { Action = action };
        }

        /// <summary>
        /// 创建一个全局 Http 配置 <see cref="System.Web.Http.HttpConfiguration"/> 的一个配置节。
        /// </summary>
        /// <param name="action">配置节配置信息</param>
        /// <returns>配置节的实例对象。</returns>
        public static IHttpConfiguration CreateHttpConfigurationFromDelegate(Action<HttpConfiguration> action)
        {
            return new DelegateHttpConfiguration { Action = action };
        }

        #endregion Public Methods

        #region Internal Classes

        internal class DelegateHttpConfiguration : IHttpConfiguration
        {
            #region Public Properties

            public Action<HttpConfiguration> Action { get; set; }

            #endregion Public Properties

            #region IHttpConfiguration 成员

            public void Configurate(HttpConfiguration configuration)
            {
                Action(configuration);
            }

            #endregion IHttpConfiguration 成员
        }

        #endregion Internal Classes

        #region Private Classes

        private class DefaultContainerConfiguration : IContainerConfiguration
        {
            #region Public Properties

            public Func<object, object> Action { get; set; }

            #endregion Public Properties

            #region IContainerConfiguration 成员

            public object ConfigurateContainer(object container)
            {
                return Action(container);
            }

            #endregion IContainerConfiguration 成员
        }

        #endregion Private Classes
    }
}