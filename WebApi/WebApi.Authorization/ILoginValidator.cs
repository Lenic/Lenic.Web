namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 登录验证器接口
    /// </summary>
    public interface ILoginValidator
    {
        /// <summary>
        /// 验证登录是否成功：<c>true</c> 表示登录成功。
        /// </summary>
        /// <param name="name">登录用户名。</param>
        /// <param name="password">登录密码。</param>
        /// <returns><c>true</c> 表示登录成功；否则返回 <c>false</c>。</returns>
        bool Validate(string name, string password);
    }
}