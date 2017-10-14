using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromUriAttribute : ParameterBindingAttribute
    {
        public static HttpAttribute DefaultInstance = new FromUriAttribute();

        public FromUriAttribute()
        {
            BindingType = "Uri";
        }
    }
}