using System.Configuration;
using System.Linq;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Reflections;
using Lenic.Web.WebApi.Services.Pipelines.Configuration;

namespace Lenic.Web.WebApi.WebHost
{
    public class ConnectionSettingsConfigHandler : IAppConfigHandler
    {
        #region IAppConfigHandler 成员

        public void Configurate(System.Configuration.Configuration[] configurations)
        {
            var meta = ((TypeX)ConfigurationManager.ConnectionStrings.GetType()).GetField("bReadOnly");
            meta.SetValue(ConfigurationManager.ConnectionStrings, false);

            configurations.SelectMany(p => p.ConnectionStrings.ConnectionStrings.OfType<ConnectionStringSettings>())
                          .Where(p => ConfigurationManager.ConnectionStrings.IndexOf(p) < 0)
                          .ForEach(ConfigurationManager.ConnectionStrings.Add);

            meta.SetValue(ConfigurationManager.ConnectionStrings, true);
        }

        #endregion IAppConfigHandler 成员
    }
}