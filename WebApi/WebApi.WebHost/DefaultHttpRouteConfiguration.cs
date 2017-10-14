using System.Web.Http;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 缺省 Http 请求路由配置类
    /// </summary>
    public class DefaultHttpRouteConfiguration : IHttpConfiguration
    {
        #region IConfiguration 成员

        /// <summary>
        /// 配置关联的 Http 配置初始化管道。
        /// </summary>
        /// <param name="configuration">需要配置的 Http 配置初始化管道。</param>
        public void Configurate(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("DefaultApi", "{namespace}/{controller}/{action}");
            configuration.Routes.MapHttpRoute("DefaultModuleApi", "{module}/{namespace}/{controller}/{action}");
        }

        #endregion IConfiguration 成员
    }
}