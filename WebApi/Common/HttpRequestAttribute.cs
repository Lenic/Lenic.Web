using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class HttpRequestAttribute : HttpAttribute
    {
        public string HttpMethod { get; protected set; }

        public HttpRequestAttribute()
        {
        }
    }
}