using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Lenic.Web.WebApi.WebHost;

namespace WebApiDemo
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            WebApiStarter.Register();

            // Error detail output configuration
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // var item = GlobalConfiguration.Configuration.MessageHandlers.FirstOrDefault(p => p.GetType() == typeof(PrincipalHandler));
            // if (item != null)
            //     GlobalConfiguration.Configuration.MessageHandlers.Remove(item);
        }
    }
}