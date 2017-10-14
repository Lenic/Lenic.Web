using System;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class EnumValueWriter : IValueWriter
    {
        public bool Handles(Type type)
        {
            return type.IsEnum;
        }

        public string Write(object value)
        {
            var enumType = value.GetType();

            return string.Format("{0}'{1}'", enumType.FullName, value);
        }
    }
}