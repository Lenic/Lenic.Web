using Lenic.Web.WebApi.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Lenic.Web.WebApi.ExtensionPoints
{
    public class ReturnValueWrapperAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
                return;

            var response = actionExecutedContext.Response;
            if (response == null)
                return;

            var obj = response.Content as ObjectContent;
            if (obj == null)
                return;

            var returnType = actionExecutedContext.ActionContext.ActionDescriptor.ReturnType;
            var controllerType = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType;

            var value = GlobalObjectProcessorX.Instance.RaiseCompletedEvent(obj.Value, controllerType, returnType, () =>
            {
                var dic = new Dictionary<string, object>();
                dic.Add("ActionName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                dic.Add("ActionArguments", actionExecutedContext.ActionContext.ActionArguments);
                dic.Add("RequestUri", actionExecutedContext.ActionContext.Request.RequestUri);
                dic.Add("RouteValues", actionExecutedContext.ActionContext.ControllerContext.RouteData.Values);
                dic.Add("RequestMethod", actionExecutedContext.ActionContext.Request.Method.Method);
                dic.Add("RequestHeaders", actionExecutedContext.ActionContext.Request.Headers);
                return new GlobalObjectParameters { DataSource = dic };
            });
            if (!ReferenceEquals(value, obj.Value))
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, value, ServiceLocator.Current.GetInstance<HttpConfiguration>());
        }
    }

    public class ReturnExceptionWrapperAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
                return;

            var controllerType = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType;

            var obj = GlobalObjectProcessorX.Instance.RaiseExceptionEvent(actionExecutedContext.Exception, controllerType, () =>
            {
                var dic = new Dictionary<string, object>();
                dic.Add("ActionName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                dic.Add("ActionArguments", actionExecutedContext.ActionContext.ActionArguments);
                dic.Add("RequestUri", actionExecutedContext.ActionContext.Request.RequestUri);
                dic.Add("RouteValues", actionExecutedContext.ActionContext.ControllerContext.RouteData.Values);
                dic.Add("RequestMethod", actionExecutedContext.ActionContext.Request.Method.Method);
                dic.Add("RequestHeaders", actionExecutedContext.ActionContext.Request.Headers);
                return new GlobalObjectParameters { DataSource = dic };
            });
            if (!ReferenceEquals(obj, null))
            {
                if (obj is HttpResponseMessage)
                    actionExecutedContext.Response = obj as HttpResponseMessage;
                else if (obj is HttpResponseException)
                    actionExecutedContext.Response = (obj as HttpResponseException).Response;
                else if (obj is Exception)
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, obj as Exception);
                else
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, obj, ServiceLocator.Current.GetInstance<HttpConfiguration>());

                if (actionExecutedContext.Response.RequestMessage == null)
                    actionExecutedContext.Response.RequestMessage = actionExecutedContext.Request;
            }
        }
    }
}