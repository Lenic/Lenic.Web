using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lenic.Framework.Common.Extensions;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Microsoft.Practices.ServiceLocation;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 身份令牌注册控制器
    /// </summary>
    public class UserTokenController : ApiController, IIdentity
    {
        /// <summary>
        /// 向服务器注册身份令牌。
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RegistToken([FromBody]string identity)
        {
            string token;
            if (ServiceLocator.Current.GetInstance<IAuthenticationTokenRegister>().Regist(identity, out token))
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(token) };
            else
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }

        #region IIdentity 成员

        /// <summary>
        /// 验证身份。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <returns><c>true</c> 表示验证成功。</returns>
        [NonAction]
        public bool Authenticate(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            if (data.Values.GetValueX("namespace", string.Empty).ToString() == "Authorization" &&
                data.Values.GetValueX("controller", string.Empty).ToString() == "UserToken" &&
                data.Values.GetValueX("action", string.Empty).ToString() == "RegistToken" &&
                request.Content.Headers.ContentLength.HasValue &&
                request.Content.Headers.ContentLength.Value > 0)
            {
                return true;
            }
            return false;
        }

        #endregion IIdentity 成员
    }
}