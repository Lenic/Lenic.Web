using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 身份授权处理节点
    /// </summary>
    public class PrincipalAuthorizationFilter : IAuthorizationFilter
    {
        #region IAuthorizationFilter 成员

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (ServiceLocator.Current.GetAllInstances<IIdentity>().Select(p => p.Authenticate(actionContext.Request)).Any(p => p))
                return continuation();

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "请求未被允许，请登录或确认有此权限！"));
            return tcs.Task;
        }

        #endregion IAuthorizationFilter 成员

        #region IFilter 成员

        public bool AllowMultiple
        {
            get { return false; }
        }

        #endregion IFilter 成员
    }
}