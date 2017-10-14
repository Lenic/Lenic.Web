using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ODataParameterAttribute : ParameterBindingAttribute
    {
        public static HttpAttribute DefaultInstance = new ODataParameterAttribute();

        public ODataParameterAttribute()
        {
            BindingType = "ODataParameter";
        }
    }
}