using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Microsoft.Practices.ServiceLocation;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 动态模块自定义配置配置类
    /// </summary>
    public class AppConfigConfiguration : IHttpConfiguration
    {
        #region IHttpConfiguration 成员

        /// <summary>
        /// 配置关联的 Http 配置初始化管道。
        /// </summary>
        /// <param name="configuration">需要配置的 Http 配置初始化管道。</param>
        public void Configurate(HttpConfiguration configuration)
        {
            var items = ServiceLocator.Current.GetAllInstances<IAppConfigHandler>().ToArray();
            if (items.Any())
            {
                var data = DynamicModules.Instance
                    .Modules
                    .Where(p => !string.IsNullOrWhiteSpace(p.Configuration.AppConfig))
                    .SelectMany(p => p.Configuration.AppConfig.Split(';').Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => Path.Combine(p.Path, r)))
                    .Select(p =>
                    {
                        try
                        {
                            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = p }, ConfigurationUserLevel.None);
                        }
                        catch (ConfigurationErrorsException ex)
                        {
                            throw new ModuleConfigException(string.Format("动态模块自定义配置文件【{0}】解析异常！", p), ex);
                        }
                    })
                    .ToArray();
                items.ForEach(p =>
                {
                    try
                    {
                        p.Configurate(data);
                    }
                    catch (Exception ex)
                    {
                        var type = p.GetType();
                        var sbMsg = new StringBuilder()
                            .AppendLine("运行动态模块自定义配置异常：")
                            .AppendFormat("异常类型全名：{0}", type.AssemblyQualifiedName)
                            .AppendLine()
                            .AppendFormat("程序集全路径为：{0}", type.Assembly.Location)
                            .AppendLine();

                        throw new ModuleConfigException(sbMsg.ToString(), ex);
                    }
                });
            }
        }

        #endregion IHttpConfiguration 成员
    }
}