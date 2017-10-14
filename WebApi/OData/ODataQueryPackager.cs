using Microsoft.Data.Edm;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace Lenic.Web.WebApi.OData
{
    public class ODataQueryPackager
    {
        private const string EdmModelPrefixKey = "Intime_EdmModel";
        private const string CreateODataQueryOptionsCtorKey = "Intime_CreateODataQueryOptionsOfT";
        private static MethodInfo _createODataQueryOptions = typeof(ODataQueryPackager).GetMethod("CreateODataQueryOptions", BindingFlags.NonPublic | BindingFlags.Static);
        private static ConstructorInfo _ctor = typeof(ODataConventionModelBuilder).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(HttpConfiguration), typeof(bool) }, null);

        private HttpParameterBinding _binding;
        private HttpActionContext _context;

        public ODataQueryPackager(HttpParameterBinding binding, HttpActionContext context)
        {
            _binding = binding;
            _context = context;
        }

        public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings = null)
        {
            var func = (Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>)_binding.Descriptor.Properties.GetOrAdd(CreateODataQueryOptionsCtorKey, _ =>
                    Delegate.CreateDelegate(typeof(Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>), _createODataQueryOptions.MakeGenericMethod(new Type[] { query.ElementType })));

            var model = GetEdmModel(_context.ActionDescriptor, query.ElementType);
            ODataQueryOptions options = func(new ODataQueryContext(model, query.ElementType), _context.Request);
            if (querySettings == null)
                return options.ApplyTo(query);
            else
                return options.ApplyTo(query, querySettings);
        }

        private static ODataQueryOptions<T> CreateODataQueryOptions<T>(ODataQueryContext context, HttpRequestMessage request)
        {
            return new ODataQueryOptions<T>(context, request);
        }

        protected internal static IEdmModel GetEdmModel(HttpActionDescriptor actionDescriptor, Type entityClrType)
        {
            return actionDescriptor.Properties.GetOrAdd(EdmModelPrefixKey + entityClrType.FullName, delegate(object _)
            {
                var builder = _ctor.Invoke(new object[] { actionDescriptor.Configuration, true }) as ODataConventionModelBuilder;
                var entityType = builder.AddEntity(entityClrType);
                builder.AddEntitySet(entityClrType.Name, entityType);
                return builder.GetEdmModel();
            }) as IEdmModel;
        }
    }
}