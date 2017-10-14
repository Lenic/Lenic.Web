using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.IO;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Helper.Help
{
    public partial class Methods : System.Web.UI.Page
    {
        public string PageTitle { get; set; }

        public MethodMetaData[] DataSource { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var controller = ServiceLocator.Current.GetInstance<IHttpControllerSelector>().GetControllerMapping().GetValueX(Request["key"], null);

            PageTitle = string.Format("控制器【{0}】的公开方法列表", controller.ControllerType.FullName);

            DataSource = LoadData(controller).ToArray();
        }

        private IEnumerable<MethodMetaData> LoadData(HttpControllerDescriptor controller)
        {
            var defaultMethodDescription = "--";
            var defaultParameterDescription = "--";

            var data = GetMethods(controller.ControllerType)
                .Where(p => !p.IsGenericMethod && !p.IsSpecialName && !p.GetCustomAttributes(typeof(NonActionAttribute), true).Any())
                .Select(p => new ReflectedHttpActionDescriptor(controller, p))
                .ToArray();
            foreach (var item in data)
            {
                var meta = new MethodMetaData
                {
                    Name = item.ActionName,
                    Title = GetMethodTitle(item.ActionName, item.MethodInfo),
                    HttpMethod = string.Join(", ", item.SupportedHttpMethods.Select(r => r.Method)),
                    ReturnType = Toolkit.GetTypeURL(item.MethodInfo.ReturnType),
                    URL = string.Format("{0}://{1}/{2}/{3}", Request.Url.Scheme, Request.Url.Authority, Request["key"].Replace('.', '/'), item.ActionName),
                };

                var parser = GetDescription(item);
                if (parser != null)
                {
                    meta.Description = parser.InnerParsers.First(p => p.Type == "summary").Text;
                    if (meta.Description != null)
                        meta.Description = meta.Description.Trim();

                    meta.Parameters = item.GetParameters().Select(p =>
                    {
                        var parameter = new ParameterMetaData
                        {
                            Id = string.Format("Parameter_{0}_{1}_{2}", item.MethodInfo.GetHashCode(), p.ParameterName, p.ParameterType.Name),
                            Name = p.ParameterName,
                            ParameterType = Toolkit.GetTypeURL(p.ParameterType),
                            BindingType = p.ParameterBinderAttribute == null ? "缺省绑定" : p.ParameterBinderAttribute.GetType().Name.Replace("Attribute", string.Empty),
                        };

                        var innerParser = parser.InnerParsers.FirstOrDefault(r => r.Type == "param" && r.GetString("name") == p.ParameterName);
                        if (innerParser != null)
                            parameter.Description = innerParser.Text.Trim();
                        else
                            parameter.Description = defaultParameterDescription;

                        return parameter;
                    }).ToArray();
                }
                else
                {
                    meta.Description = defaultMethodDescription;

                    meta.Parameters = item.GetParameters().Select(p => new ParameterMetaData
                    {
                        Id = string.Format("Parameter_{0}_{1}_{2}", item.MethodInfo.GetHashCode(), p.ParameterName, p.ParameterType.Name),
                        Name = p.ParameterName,
                        ParameterType = Toolkit.GetTypeURL(p.ParameterType),
                        BindingType = p.ParameterBinderAttribute == null ? "缺省绑定" : p.ParameterBinderAttribute.GetType().Name.Replace("Attribute", string.Empty),
                        Description = defaultParameterDescription,
                    }).ToArray();
                }

                yield return meta;
            }
        }

        private XmlParser GetDescription(ReflectedHttpActionDescriptor method)
        {
            string prefix = string.Empty;
            if (method.MethodInfo.DeclaringType.IsGenericType)
                prefix = method.MethodInfo.DeclaringType.GetGenericTypeDefinition().FullName;
            else
                prefix = method.MethodInfo.DeclaringType.FullName;

            var name = string.Format("M:{0}.{1}", prefix, method.MethodInfo.Name);
            var parameters = method.MethodInfo.GetParameters();
            if (parameters.Any())
            {
                if (method.MethodInfo.DeclaringType.IsGenericType)
                {
                    var baseMethodMeta = MethodInfo.GetMethodFromHandle(method.MethodInfo.MethodHandle, method.MethodInfo.DeclaringType.GetGenericTypeDefinition().TypeHandle);
                    parameters = baseMethodMeta.GetParameters();
                }

                int index = 0;
                name = string.Format("{0}({1})", name, string.Join(",", parameters.Select(p =>
                {
                    if (p.ParameterType.FullName != null)
                        return Toolkit.GetTypeName(p.ParameterType, false, Tuple.Create('{', '}'));

                    return string.Format("`{0}", index++);
                })));
            }

            var parser = Toolkit.GetParser(method.MethodInfo.DeclaringType);
            if (parser == null)
                return null;

            var node = parser.InnerParsers
                .First(p => p.Type == "members")
                .InnerParsers
                .FirstOrDefault(p => p.Type == "member" && p.GetString("name", null, XmlParserTypes.Attribute) == name);

            if (node != null)
                return node;
            else
                return null;
        }

        private string GetMethodTitle(string actionName, MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Any())
                return string.Format("{0}({1})", actionName, string.Join(",", method.GetParameters().Select(p => Toolkit.GetTypeName(p.ParameterType))));
            else
                return actionName;
        }

        private IEnumerable<MethodInfo> GetMethods(Type type)
        {
            if (type != typeof(ApiController))
            {
                foreach (var item in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return item;

                foreach (var item in GetMethods(type.BaseType))
                    yield return item;
            }
        }

        private string GetTypeInfoURL(Type type)
        {
            return string.Format("{0}://{1}/Help/Classes.aspx?typeFullName={2}", Request.Url.Scheme, Request.Url.Authority, Server.UrlEncode(Toolkit.GetTypeNameString(type)));
        }

        #region Inner Model Class

        public class MethodMetaData
        {
            public string Name { get; set; }

            public string Title { get; set; }

            public string HttpMethod { get; set; }

            public TypeURL ReturnType { get; set; }

            public string Description { get; set; }

            public string URL { get; set; }

            public ParameterMetaData[] Parameters { get; set; }
        }

        public class ParameterMetaData
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public TypeURL ParameterType { get; set; }

            public string BindingType { get; set; }

            public string Description { get; set; }
        }

        #endregion Inner Model Class
    }
}