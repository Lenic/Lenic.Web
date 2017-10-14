namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 身份令牌注册器接口
    /// </summary>
    public interface IAuthenticationTokenRegister
    {
        /// <summary>
        /// 向指定用户注册一个新的身份令牌。
        /// </summary>
        /// <param name="identity">用户的唯一标识。</param>
        /// <param name="token">产生的身份令牌。</param>
        /// <returns><c>true</c> 表示注册成功；否则返回 <c>false</c> 。</returns>
        bool Regist(string identity, out string token);
    }
}