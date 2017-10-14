using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Lenic.Web.WebApi.OData
{
    public class ODataBindingAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            return new ODataQueryParameterBinding(parameter);
        }

        internal class ODataQueryParameterBinding : HttpParameterBinding
        {
            public ODataQueryParameterBinding(HttpParameterDescriptor parameterDescriptor)
                : base(parameterDescriptor)
            {
            }

            public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
            {
                if (actionContext == null)
                    throw new ArgumentNullException("[ODataBindingAttribute].[ODataQueryParameterBinding].[ExecuteBindingAsync].actionContext");

                var packager = new ODataQueryPackager(this, actionContext);
                base.SetValue(actionContext, packager);

                TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
                tcs.SetResult(0);
                return tcs.Task;
            }
        }
    }
}