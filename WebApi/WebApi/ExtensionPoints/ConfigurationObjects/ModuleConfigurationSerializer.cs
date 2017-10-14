using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lenic.Framework.Common.Extensions;

namespace Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects
{
    /// <summary>
    /// 动态运行模块配置序列化类
    /// </summary>
    public class ModuleConfigurationSerializer
    {
        /// <summary>
        /// 获取动态运行模块配置版本信息的缺省值。
        /// </summary>
        public static readonly Version DefaultVersion = new Version(0, 0, 0, 0);

        /// <summary>
        /// 序列化一个
        /// <see cref="Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects.ModuleConfiguration"/>
        /// 类的实例对象。
        /// </summary>
        /// <param name="config">一个动态运行模块配置信息的实例对象。</param>
        /// <returns>序列化完成后的可编辑 Xml 容器实例对象。</returns>
        public XElement Serialize(ModuleConfiguration config)
        {
            var result = new XElement(typeof(ModuleConfiguration).Name);

            result.SetAttributeValue("Enabled", config.Enabled);

            result.SetElementValue("Index", config.Index);
            result.SetElementValue("Name", config.Name);
            result.Add(new XElement("Description", new XCData(config.Description)));
            result.SetElementValue("AssemblyDirectory", config.AssemblyDirectory);
            result.SetElementValue("AppConfig", config.AppConfig);
            result.SetElementValue("Version", config.Version);

            return result;
        }

        /// <summary>
        /// 反序列化一个 <see cref="System.Xml.Linq.XElement" /> 类的实例对象。
        /// </summary>
        /// <param name="node">包含可反序列化信息的一个可编辑 Xml 容器实例对象。</param>
        /// <returns>反序列化后的动态运行模块配置信息的实例对象。</returns>
        public ModuleConfiguration Deserialize(XElement node)
        {
            var result = new ModuleConfiguration();

            result.Enabled = node.Attribute("Enabled").IfNull(false, p => p.Value.ChangeType<bool>(true, true));

            result.Index = node.Element("Index").IfNull(0, p => p.Value.ChangeType<int>(0, true));
            result.Name = node.Element("Name").IfNull(string.Empty, p => p.Value.ChangeType<string>(string.Empty, true));
            result.Description = node.Element("Description").IfNull(string.Empty, p => p.Value.ChangeType<string>(string.Empty, true));
            result.AssemblyDirectory = node.Element("AssemblyDirectory").Value;
            result.AppConfig = node.Element("AppConfig").Value;
            result.Version = node.Element("Name").IfNull(DefaultVersion, p =>
            {
                Version version = null;
                if (Version.TryParse(p.Value, out version))
                    return version;
                else
                    return DefaultVersion;
            });

            return result;
        }
    }
}