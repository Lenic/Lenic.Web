using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HttpGetAttribute : HttpRequestAttribute
    {
        public HttpGetAttribute()
        {
            HttpMethod = "GET";
        }
    }
}