using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lenic.Web.WebApi.Services.Pipelines.Configuration
{
    /// <summary>
    /// 程序启动配置管道
    /// </summary>
    public static class ConfigurationPipeline
    {
        /// <summary>
        /// IOC 容器配置管道
        /// </summary>
        public static IList<IContainerConfiguration> ContainerConfiguration { get; private set; }

        /// <summary>
        /// 全局 HttpConfiguration 配置管道
        /// </summary>
        public static IList<IHttpConfiguration> HttpConfiguration { get; private set; }

        /// <summary>
        /// 初始化 <see cref="ConfigurationPipeline"/> 类。
        /// </summary>
        static ConfigurationPipeline()
        {
            ContainerConfiguration = new Collection<IContainerConfiguration>();
            HttpConfiguration = new Collection<IHttpConfiguration>();
        }
    }
}