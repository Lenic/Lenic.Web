using System;

namespace Lenic.Web.WebApi.HttpBinding
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ActionNameAttribute : HttpAttribute
    {
        public string Value { get; set; }

        public ActionNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("[ActionNameAttribute].[ctor].value", "[ActionNameAttribute].[ctor].value 不能为 null ！");
            Value = value;
        }
    }
}