using System;
using System.Net.Http;
using System.Text;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Microsoft.Practices.ServiceLocation;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 缺省身份验证程序
    /// </summary>
    public class DefaultIdentity : IIdentity
    {
        #region IIdentity 成员

        /// <summary>
        /// 验证身份。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <returns><c>true</c> 表示验证成功。</returns>
        public bool Authenticate(HttpRequestMessage request)
        {
            var authorization = request.Headers.Authorization;
            if (authorization != null && authorization.Scheme.Equals("Intime", StringComparison.CurrentCultureIgnoreCase))
            {
                var value = authorization.Parameter;
                if (value.IsNullOrWhiteSpace())
                    return false;

                value = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                var validator = ServiceLocator.Current.GetInstance<IAuthenticationTokenValidator>();
                if (validator == null)
                    return false;

                if (validator.Validate(value))
                {
                    var provider = ServiceLocator.Current.GetInstance<IPermissionProvider>();
                    if (provider == null)
                        return false;

                    return provider.Ask(request);
                }
            }
            return false;
        }

        #endregion IIdentity 成员
    }
}