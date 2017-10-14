using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Xml.Linq;

namespace Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects
{
    /// <summary>
    /// 动态运行模块类
    /// </summary>
    public sealed class DynamicModules
    {
        #region Private Fields

        private readonly ModuleConfigurationSerializer _serializer;

        #endregion Private Fields

        #region Static Fields

        /// <summary>
        /// 获取动态运行模块的配置说明文件名称。
        /// </summary>
        public static readonly string DefaultDescriptorFileName = "PlugInConfig.xml";

        /// <summary>
        /// 获取动态运行模块类的唯一实例。
        /// </summary>
        public static readonly DynamicModules Instance = new DynamicModules();

        #endregion Static Fields

        #region Business Properties

        /// <summary>
        /// 获取当前程序的运行环境（Web 网站，还是本地应用程序）。
        /// </summary>
        public ProgramTypes ProgramType { get; private set; }

        /// <summary>
        /// 获取当前程序运行的基路径（Web 网站为 bin 目录，本地应用为 exe 文件所在目录）。
        /// </summary>
        public string BaseDirectory { get; private set; }

        /// <summary>
        /// 获取已经加载的所有动态模块信息列表。
        /// </summary>
        public ModuleInfo[] Modules { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="DynamicModules" /> 类的实例对象。
        /// </summary>
        private DynamicModules()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ProgramType = ProgramTypes.App;
            if (!AppDomain.CurrentDomain.FriendlyName.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase))
            {
                BaseDirectory = Path.Combine(BaseDirectory, "bin");
                ProgramType = ProgramTypes.Web;
            }

            _serializer = new ModuleConfigurationSerializer();
            Modules = Directory.GetDirectories(BaseDirectory, "*", SearchOption.TopDirectoryOnly)
                .Where(p => File.Exists(Path.Combine(p, DefaultDescriptorFileName)))
                .Select(GetModuleInfo)
                .Where(p => p.Configuration.Enabled)
                .OrderBy(p => p.Configuration.Index)
                .ToArray();
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 初始化构建动态模块程序集。
        /// </summary>
        public void BuildAssemblies()
        {
            Modules.SelectMany(p => p.AssemblyNames.Select(r => Tuple.Create(r, p)))
                .GroupBy(p => p.Item1.Name)
                .Select(p => p.OrderByDescending(r => r.Item1.Version).First())
                .ForEach(p => p.Item2.LoadedAssemblies.Add(Assembly.Load(p.Item1)));

            var value = GetReferencedAssemblies()
                .OfType<Assembly>()
                .Select(p => Tuple.Create(p, p.GetName(), true)) // 返回的三个参数作用为：程序集元数据、程序集名称元数据和是否来源于运行时框架（True 表示来源于框架）
                .Concat(Modules.SelectMany(p => p.LoadedAssemblies).Select(p => Tuple.Create(p, p.GetName(), false)))
                .GroupBy(p => p.Item2.Name)
                .Select(p =>
                {
                    var data = p.OrderByDescending(r => r.Item2.Version);

                    // 第一个参数表示程序集名称字符串
                    // 第二个参数为版本最高的程序集：包含程序集元数据、程序集名称元数据和是否来自于运行时框架（True 表示来自运行时框架）
                    // 第三个参数表示此组是否包含来自运行时框架的程序集（True 表示包含来自框架的程序集）
                    return Tuple.Create(p.Key, data.First(), data.Any(r => r.Item3));
                })
                .ToArray();

            // 在有来自运行时框架的程序集，并且最新版本的程序集来自于某动态运行时模块时，抛出异常：不允许动态运行模块中程序集的版本比运行时框架高
            var errors = value.Where(p => p.Item3 && !p.Item2.Item3).ToArray();
            if (errors.Any())
            {
                var message = string.Format("发现动态运行模块中出现了比框架更高版本的程序集，具体信息请参见 Data 属性：{0}", string.Join(", ", errors.Select(p => p.Item1)));
                throw new ModuleConfigException(message).Setup(p => errors.ForEach(r => p.Data.Add(r.Item1, r.Item2.Item2.CodeBase)));
            }

            value.Where(p => !p.Item2.Item3).ForEach(p => BuildManager.AddReferencedAssembly(p.Item2.Item1));
        }

        #endregion Business Methods

        #region Private Methods

        private ModuleInfo GetModuleInfo(string configFileFullName)
        {
            XElement config;
            try
            {
                config = XDocument.Load(Path.Combine(configFileFullName, DefaultDescriptorFileName)).Root;
            }
            catch (Exception ex)
            {
                throw new ModuleConfigException(string.Format("加载模块配置文件【{0}】异常！", configFileFullName.Substring(BaseDirectory.Length)), ex);
            }

            ModuleConfiguration module;
            try
            {
                module = _serializer.Deserialize(config);
            }
            catch (Exception ex)
            {
                throw new ModuleConfigException(string.Format("反序列化配置【{0}】异常！", configFileFullName.Substring(BaseDirectory.Length)), ex);
            }

            var result = new ModuleInfo
            {
                Path = configFileFullName,
                Configuration = module
            };

            string path;
            try
            {
                path = Path.Combine(result.Path, result.Configuration.AssemblyDirectory);
            }
            catch (Exception ex)
            {
                throw new ModuleConfigException(string.Format("程序集目录【AssemblyDirectory】配置错误：{0}！", result.Path), ex);
            }

            if (result.Configuration.Enabled)
                result.AssemblyNames = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories).Select(r =>
                {
                    try
                    {
                        return AssemblyName.GetAssemblyName(r);
                    }
                    catch (Exception ex)
                    {
                        throw new ModuleConfigException(string.Format("加载程序集文件【{0}】异常！", r.Substring(BaseDirectory.Length), ex));
                    }
                }).ToArray();
            else
                result.AssemblyNames = new AssemblyName[0];

            return result;
        }

        private Assembly[] GetReferencedAssemblies()
        {
            return Directory.GetFiles(BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                            .Where(File.Exists)
                            .Select(AssemblyName.GetAssemblyName)
                            .Select(Assembly.Load)
                            .ToArray();
        }

        #endregion Private Methods
    }
}