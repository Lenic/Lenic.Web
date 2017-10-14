using Lenic.Web.WebApi.ExtensionPoints.ConfigurationObjects;

namespace Lenic.Web.WebApi.WebHost
{
    public static class Forbidden
    {
        public static void OnPreApplicationStartMethod()
        {
            DynamicModules.Instance.BuildAssemblies();
        }
    }
}