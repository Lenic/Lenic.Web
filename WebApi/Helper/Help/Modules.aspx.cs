using System;
using System.Linq;
using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;

namespace Helper.Help
{
    public partial class Modules : System.Web.UI.Page
    {
        public Tuple<string, string, string>[] DataSource { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataSource = DynamicModules.Instance.Modules.Select(p => Tuple.Create(
                p.Configuration.Name,
                p.Configuration.Description ?? "无描述信息",
                string.Format("{0}://{1}/Help/Controllers.aspx?module={2}", Request.Url.Scheme, Request.Url.Authority, p.Configuration.Name))).ToArray();
        }
    }
}