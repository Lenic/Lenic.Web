using System;
using System.Collections.Generic;

namespace Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects
{
    /// <summary>
    /// 动态运行模块配置类
    /// </summary>
    public class ModuleConfiguration
    {
        /// <summary>
        /// 获取或设置加载动态运行模块的次序索引。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的模块名称：此处因为要注册到 URL 中，所以必须为英文。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的友好性描述文本。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的可加载程序集目录（相对路径）。
        /// </summary>
        public string AssemblyDirectory { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的自定义配置文件路径（相对路径）。
        /// </summary>
        public string AppConfig { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的版本信息。
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// 获取或设置加载动态运行模块的可用性：<c>true</c> 表示可用。
        /// </summary>
        public bool Enabled { get; set; }
    }
}