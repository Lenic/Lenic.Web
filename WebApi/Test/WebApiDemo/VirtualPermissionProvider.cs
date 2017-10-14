using System.Net.Http;
using Lenic.Web.WebApi.Authorization;

namespace WebApiDemo
{
    public class VirtualPermissionProvider : IPermissionProvider
    {
        #region IPermissionProvider 成员

        public bool Ask(HttpRequestMessage request)
        {
            return true;
        }

        #endregion IPermissionProvider 成员
    }
}