namespace Lenic.Web.WebApi.Services.Pipelines.Configuration
{
    /// <summary>
    /// 动态运行模块个性化配置信息处理接口
    /// </summary>
    public interface IAppConfigHandler
    {
        /// <summary>
        /// 配置动态运行模块的个性化配置信息。
        /// </summary>
        /// <param name="configurations">动态运行模块的个性化配置信息列表。</param>
        void Configurate(System.Configuration.Configuration[] configurations);
    }
}