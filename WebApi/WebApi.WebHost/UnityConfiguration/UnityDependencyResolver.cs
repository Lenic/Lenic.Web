using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace Lenic.Web.WebApi.WebHost.UnityConfiguration
{
    internal class UnityDependencyResolver : UnityDependencyScope, IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
            : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            return new UnityDependencyScope(Container.CreateChildContainer());
        }
    }
}