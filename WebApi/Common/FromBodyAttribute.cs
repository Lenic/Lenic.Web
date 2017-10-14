using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromBodyAttribute : ParameterBindingAttribute
    {
        public static HttpAttribute DefaultInstance = new FromBodyAttribute();

        public FromBodyAttribute()
        {
            BindingType = "Body";
        }
    }
}