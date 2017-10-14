using System.Net;
using System.Net.Http;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// Http 状态码策略接口
    /// </summary>
    public interface IHttpStatusCodePolicy
    {
        /// <summary>
        /// 获取当前策略对应的 Http 状态码。
        /// </summary>
        HttpStatusCode Code { get; }

        /// <summary>
        /// 执行当前策略，并获取返回值。
        /// </summary>
        /// <param name="response">Http 直接的响应消息体。</param>
        /// <returns>经过当前策略处理后的返回值。</returns>
        object Execute(HttpResponseMessage response);
    }
}