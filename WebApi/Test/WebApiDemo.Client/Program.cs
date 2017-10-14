using Lenic.Framework.Common.Messaging;
using Lenic.Web.WebApi.Client;
using Lenic.Web.WebApi.Client.Http;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using WebApiDemo.PlugIn.Models;
using WebApiDemo.PlugIn.Models.Api;

namespace WebApiDemo.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Initialize();

            var service = ServiceLocator.Current.GetInstance<IPersonService>();

            try
            {
                //var person = service.GetById(2, new string[] { "33", "34" });

                var data = service.GetAll().ToArray();

                //Expression<Func<IQueryable<Person>, IQueryable<Person>>> expr = p => p.Where(r => r.Id >= 2);
                var expr = QueryContext.CreateQuery<Person>().Where(p => p.Id == 2).Query();
                var dd = service.GetByOData(expr);

                Expression<Func<IQueryable<Person>, Person>> expr1 = p => p.Where(r => r.Id >= 2).FirstOrDefault();
                var dd1 = service.GetByOData(expr1);

                Expression<Func<IQueryable<Person>, IQueryable<Person>>> expr2 = p => p.Where(r => r.Id >= 2);
                var dd2 = service.CountByOData(expr2);

                Expression<Func<IQueryable<Person>, int>> expr3 = p => p.Where(r => r.Id >= 2).Count();
                var dd3 = service.CountByOData(expr3);

                Expression<Func<IQueryable<Person>, int>> expr6 = p => p.Where(r => r.Id >= 10).Count();
                var dd6 = service.CountByOData(expr6);

                Expression<Func<IQueryable<Person>, IQueryable<Person>>> expr4 = p => p.Where(r => r.Id >= 2);
                var dd4 = service.AnyByOData(expr4);

                Expression<Func<IQueryable<Person>, bool>> expr5 = p => p.Where(r => r.Id >= 2).Any();
                var dd5 = service.AnyByOData(expr5);

                Expression<Func<IQueryable<Person>, int>> expr7 = p => p.Where(r => r.Id >= 10).Count();
                var dd7 = service.AnyByOData(expr7);
            }
            catch (Exception ex)
            {
                var dd = JsonConvert.SerializeObject(ex, Formatting.Indented);
            }
        }

        private static void Initialize()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IMessageRouter>(new MessageRouter());

            HttpServiceLocator.Namespaces = new string[] { "WebApiDemo.PlugIn.Models" };
            HttpServiceLocator.DefaultInstance.GetHttpClient = p => new HttpClient() { BaseAddress = new Uri("http://localhost:54619/PersonModule/") };
            HttpServiceLocator.DefaultInstance.GetValues = p => container.ResolveAll(p).ToArray();
            HttpServiceLocator.DefaultInstance.RegisterValue = (x, y, z) => container.RegisterInstance(x, y, z);
            HttpServiceLocator.DefaultInstance.TryGetValue = delegate(Type serviceType, string name, out object value)
            {
                value = null;
                if (container.IsRegistered(serviceType, name))
                {
                    value = container.Resolve(serviceType, name);
                    return true;
                }

                return false;
            };

            ServiceLocator.SetLocatorProvider(() => HttpServiceLocator.DefaultInstance);
        }
    }
}