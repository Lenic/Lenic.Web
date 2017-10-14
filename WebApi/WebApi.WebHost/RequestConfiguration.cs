using System.Linq;
using System.Web.Http;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Microsoft.Practices.ServiceLocation;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// Http 请求管道类
    /// </summary>
    public class RequestConfiguration : IHttpConfiguration
    {
        #region IHttpConfiguration 成员

        /// <summary>
        /// 配置 Http 请求处理管道。
        /// </summary>
        /// <param name="configuration">需要配置的 Http 配置初始化管道。</param>
        public void Configurate(HttpConfiguration configuration)
        {
            ServiceLocator.Current.GetAllInstances<IndexableHandler>().OrderBy(p => p.Index).ForEach(configuration.MessageHandlers.Add);
        }

        #endregion IHttpConfiguration 成员
    }
}