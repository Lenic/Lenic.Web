namespace Lenic.Web.WebApi.Services.Pipelines.Configuration
{
    /// <summary>
    /// IOC/DI 容器配置接口
    /// </summary>
    public interface IContainerConfiguration
    {
        /// <summary>
        /// 配置 IOC/DI 容器。
        /// </summary>
        /// <param name="container">用于向其配置的 IOC/DI 容器实例对象：为空时需要创建一个并返回。</param>
        /// <returns>配置完成的 IOC/DI 容器实例对象。</returns>
        object ConfigurateContainer(object container);
    }
}