using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace Lenic.Web.WebApi.WebHost.UnityConfiguration
{
    internal class UnityDependencyScope : IDependencyScope, IDisposable
    {
        public UnityDependencyScope(IUnityContainer container)
        {
            Container = container;
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public object GetService(Type serviceType)
        {
            if (typeof(IHttpController).IsAssignableFrom(serviceType))
                return Container.Resolve(serviceType);
            try
            {
                return Container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }

        protected IUnityContainer Container { get; private set; }
    }
}