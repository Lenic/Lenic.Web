using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lenic.Framework.Common.IO;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;
using Microsoft.Practices.ServiceLocation;

namespace Helper.Help
{
    public partial class Controllers : System.Web.UI.Page
    {
        public string PageTitle { get; set; }

        public string ModuleName { get; set; }

        public Tuple<string, string, string>[] DataSource { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModuleName = Request["module"];
            var module = DynamicModules.Instance.Modules.First(p => p.Configuration.Name == ModuleName);

            PageTitle = string.Format("模块【{0}】的控制器列表", module.Configuration.Name);

            DataSource = ServiceLocator.Current.GetInstance<IHttpControllerSelector>()
                .GetControllerMapping()
                .Where(p => p.Key.StartsWith(ModuleName) && !(p.Value.ControllerType.Namespace.StartsWith("System") || p.Value.ControllerType.Namespace.StartsWith("Microsoft")))
                .Select(p => Tuple.Create(p.Value.ControllerType.FullName, GetDescription(p.Value.ControllerType), GetData(p.Key)))
                .ToArray();
        }

        private string GetDescription(Type controllerType)
        {
            var defaultDescription = "未找到描述信息";
            var name = string.Format("T:{0}", controllerType.FullName);

            var parser = Toolkit.GetParser(controllerType);
            if (parser == null)
                return defaultDescription;

            var node = parser.InnerParsers
                .First(p => p.Type == "members")
                .InnerParsers
                .FirstOrDefault(p => p.Type == "member" && p.GetString("name", defaultDescription, XmlParserTypes.Attribute) == name);

            if (node != null)
                return node.InnerParsers.FirstOrDefault(p => p.Type == "summary").IfNull(defaultDescription, p => p.Text).Trim();
            else
                return defaultDescription;
        }

        private string GetData(string key)
        {
            return string.Format("{0}://{1}/Help/Methods.aspx?key={2}", Request.Url.Scheme, Request.Url.Authority, key);
        }
    }
}