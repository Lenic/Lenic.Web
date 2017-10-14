using System.Net.Http;
using System.Text;
using Lenic.Framework.Common.Security;
using Lenic.Web.WebApi.Authorization;
using Lenic.Web.WebApi.Authorization.RequestValidator.Properties;
using Lenic.Web.WebApi.Client;

namespace Lenic.Web.WebApi.Authorization.RequestValidator
{
    public class PasswordLoginValidator : ILoginValidator
    {
        private static readonly RsaHelper Encoder = new RsaHelper(Resources.RsaKey);

        public string ValidateURL { get; set; }

        #region ILoginValidator 成员

        public bool Validate(string name, string password)
        {
            using (var client = new HttpClient())
            {
                var content = Encoder.Encrypt(string.Format("{0}:{1}", name, password));

                return client.FetchValueByPost<bool>(content, ValidateURL);
            }
        }

        #endregion ILoginValidator 成员
    }
}