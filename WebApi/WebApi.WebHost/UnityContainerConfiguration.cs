using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// IOC/DI 容器配置类
    /// </summary>
    public class UnityContainerConfiguration : IContainerConfiguration
    {
        #region IIocConfiguration 成员

        /// <summary>
        /// 配置 IOC/DI 容器。
        /// </summary>
        /// <param name="container">用于向其配置的 IOC/DI 容器实例对象：为空时需要创建一个并返回。</param>
        /// <returns>配置完成的 IOC/DI 容器实例对象。</returns>
        /// <exception cref="System.ArgumentNullException">[UnityConfiguration].[ConfigurateContainer].container</exception>
        ///
        ///
        /// <exception cref="System.ArgumentException">[UnityConfiguration].[ConfigurateContainer].container
        /// is not IUnityContainer instance.</exception>
        public object ConfigurateContainer(object container)
        {
            if (ReferenceEquals(container, null))
                throw new ArgumentNullException("[UnityConfiguration].[ConfigurateContainer].container");
            if (!(container is IUnityContainer))
                throw new ArgumentException("[UnityConfiguration].[ConfigurateContainer].container is not IUnityContainer instance.");

            var path = "Configuration\\unity.config";
            return DynamicModules.Instance
                .Modules
                .Select(p => Path.Combine(p.Path, p.Configuration.AssemblyDirectory, path))
                .ToList()
                .Setup(p => p.Insert(0, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
                .Where(File.Exists)
                .Aggregate(container as IUnityContainer, (c, p) =>
                {
                    try
                    {
                        var map = new ExeConfigurationFileMap { ExeConfigFilename = p };
                        var section = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None).GetSection("unity") as UnityConfigurationSection;

                        return section.Configure(c);
                    }
                    catch (Exception ex)
                    {
                        throw new ModuleConfigException(string.Format("加载 Unity 配置出现异常，具体路径为：{0}", p), ex);
                    }
                });
        }

        #endregion IIocConfiguration 成员
    }
}