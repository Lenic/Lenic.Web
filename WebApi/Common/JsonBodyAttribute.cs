using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class JsonBodyAttribute : ParameterBindingAttribute
    {
        public JsonBodyAttribute()
        {
            BindingType = "JsonBody";
        }
    }
}