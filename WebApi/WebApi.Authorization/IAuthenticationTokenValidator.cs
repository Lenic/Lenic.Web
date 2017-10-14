using System;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 身份令牌验证器接口
    /// </summary>
    public interface IAuthenticationTokenValidator : IAuthenticationTokenRegister
    {
        /// <summary>
        /// 获取或设置滑动过期的间隔
        /// </summary>
        TimeSpan SlideDue { get; set; }

        /// <summary>
        /// 验证身份令牌是否通过：<c>true</c> 表示验证通过。
        /// </summary>
        /// <param name="token">待验证的身份令牌。</param>
        /// <returns><c>true</c> 表示验证通过，否则返回 <c>false</c> 。</returns>
        bool Validate(string token);
    }
}