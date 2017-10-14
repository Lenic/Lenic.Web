using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 程序集解析扩展配置类
    /// </summary>
    public class AssemblyResolveConfiguration : IContainerConfiguration
    {
        #region Private Fields

        private IDictionary<string, Assembly> _nameCache = null;
        private IDictionary<string, Assembly> _fullNameCache = null;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="AssemblyResolveConfiguration"/> 类的实例对象。
        /// </summary>
        public AssemblyResolveConfiguration()
        {
            var data = DynamicModules.Instance.Modules.SelectMany(p => p.LoadedAssemblies);
            _nameCache = data.GroupBy(p => p.GetName().Name)
                             .ToDictionary(p => p.Key, p => p.OrderByDescending(r => r.GetName().Version, VersionComparer.Instance).First());
            _fullNameCache = data.GroupBy(p => p.FullName)
                             .ToDictionary(p => p.Key, p => p.OrderByDescending(r => r.GetName().Version, VersionComparer.Instance).First());
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #endregion Entrance

        #region IContainerConfiguration 成员

        /// <summary>
        /// 配置 IOC/DI 容器所需的额外程序集解析事件。
        /// </summary>
        /// <param name="container">用于向其配置的 IOC/DI 容器实例对象：为空时需要创建一个并返回。</param>
        /// <returns>配置完成的 IOC/DI 容器实例对象。</returns>
        public object ConfigurateContainer(object container)
        {
            return container;
        }

        #endregion IContainerConfiguration 成员

        #region Private Methods

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly singleResult = null;
            if (_fullNameCache.TryGetValue(args.Name, out singleResult))
                return singleResult;

            var name = new AssemblyName(args.Name);
            if (_nameCache.TryGetValue(name.Name, out singleResult))
                return singleResult;

            return null;
        }

        #endregion Private Methods

        #region Private Class : VersionComparer

        private class VersionComparer : IComparer<Version>
        {
            public static VersionComparer Instance = new VersionComparer();

            #region IComparer<Version> 成员

            public int Compare(Version x, Version y)
            {
                return x > y ? 1 : (x == y ? 0 : -1);
            }

            #endregion IComparer<Version> 成员
        }

        #endregion Private Class : VersionComparer
    }
}