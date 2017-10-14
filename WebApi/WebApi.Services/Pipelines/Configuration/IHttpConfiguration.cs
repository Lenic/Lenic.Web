using System.Web.Http;

namespace Lenic.Web.WebApi.Services.Pipelines.Configuration
{
    /// <summary>
    /// Http 配置初始化管道接口
    /// </summary>
    public interface IHttpConfiguration
    {
        /// <summary>
        /// 配置关联的 Http 配置初始化管道。
        /// </summary>
        /// <param name="configuration">需要配置的 Http 配置初始化管道。</param>
        void Configurate(HttpConfiguration configuration);
    }
}