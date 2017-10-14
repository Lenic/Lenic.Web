using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ParameterBindingAttribute : HttpAttribute
    {
        public string BindingType { get; protected set; }

        public ParameterBindingAttribute()
        {
        }
    }
}