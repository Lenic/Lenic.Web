using System.Net.Http;

namespace Lenic.Web.WebApi.Client.Http
{
    /// <summary>
    /// Http 协议代理对象创建者接口
    /// </summary>
    public interface IHttpProxy
    {
        /// <summary>
        /// 获取或设置用于发送 Http 请求的 Http 客户端实例对象。
        /// </summary>
        HttpClient ClientProxy { get; set; }

        /// <summary>
        /// 构建一个代理 Http 请求的代理对象。
        /// </summary>
        /// <returns></returns>
        object BuildProxyInstance();
    }
}