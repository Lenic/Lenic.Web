using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http.Routing;

namespace Lenic.Web.WebApi
{
    internal static class Toolkit
    {
        public static string GetRouteVariable(this IHttpRouteData routeData, string name)
        {
            object result = null;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (string)result;
            }
            return null;
        }

        public static T IfNull<TSource, T>(this TSource source, T trueResult, Func<TSource, T> falseFunc) where TSource : class
        {
            return source == null ? trueResult : falseFunc(source);
        }

        public static T GetInstance<T>(string name = null)
        {
            try
            {
                if (name == null)
                    return ServiceLocator.Current.GetInstance<T>();
                else
                    return ServiceLocator.Current.GetInstance<T>(name);
            }
            catch (Exception ex)
            {
                throw new ModuleConfigException(string.Format("解析名称为【{0}】的类型【{1}】出现异常！", name, typeof(T).FullName), ex);
            }
        }
    }
}