using Lenic.Web.WebApi.Authorization;

namespace Lenic.Web.WebApi.Authorization.RequestValidator
{
    public class DomainLoginValidator : ILoginValidator
    {
        #region ILoginValidator 成员

        public bool Validate(string name, string password)
        {
            return true;
        }

        #endregion ILoginValidator 成员
    }
}