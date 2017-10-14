using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Lenic.Web.WebApi.HttpBinding
{
    public class HttpAttribute : Attribute
    {
    }

    public static class HttpAttributeExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo element) where T : HttpAttribute
        {
            var data = Attribute.GetCustomAttributes(element, typeof(T), false);
            if (data.Any())
                return (T)data.First();
            else
                return null;
        }

        public static T GetCustomAttribute<T>(this ParameterInfo element) where T : HttpAttribute
        {
            var data = Attribute.GetCustomAttributes(element, typeof(T), false);
            if (data.Any())
                return (T)data.First();

            if (IsCLRType(element.ParameterType))
                return FromUriAttribute.DefaultInstance as T;
            else if (element.ParameterType == typeof(object))
                return ODataParameterAttribute.DefaultInstance as T;

            return null;
        }

        private static bool IsCLRType(Type type)
        {
            var targetType = Nullable.GetUnderlyingType(type);
            if (targetType != null)
                type = targetType;

            var code = Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                    return false;

                default:
                    return false;
            }
        }
    }
}