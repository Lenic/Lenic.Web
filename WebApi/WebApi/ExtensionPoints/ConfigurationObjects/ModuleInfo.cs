using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects
{
    /// <summary>
    /// 动态运行模块信息类
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 获取或设置动态运行模块信息文件夹的绝对路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取或设置动态运行模块信息的配置信息。
        /// </summary>
        public ModuleConfiguration Configuration { get; set; }

        /// <summary>
        /// 获取或设置动态运行模块信息的关联程序集列表信息。
        /// </summary>
        public ICollection<Assembly> LoadedAssemblies { get; set; }

        internal AssemblyName[] AssemblyNames { get; set; }

        /// <summary>
        /// 初始化新建一个 <see cref="Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects.ModuleInfo"/> 类的实例对象。
        /// </summary>
        public ModuleInfo()
        {
            LoadedAssemblies = new Collection<Assembly>();
        }
    }
}