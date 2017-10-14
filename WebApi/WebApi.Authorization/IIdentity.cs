using System.Net.Http;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 身份验证接口
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// 验证身份。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <returns><c>true</c> 表示验证成功。</returns>
        bool Authenticate(HttpRequestMessage request);
    }
}