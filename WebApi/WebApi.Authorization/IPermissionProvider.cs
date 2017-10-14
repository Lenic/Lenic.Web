using System.Net.Http;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 权限提供者接口
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// 获取或设置一个值，通过该值指示当前请求消息是否通过权限验证：<c>true</c> 表示通过权限验证。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <returns><c>true</c> 表示通过权限验证；否则返回 <c>false</c> 。</returns>
        bool Ask(HttpRequestMessage request);
    }
}