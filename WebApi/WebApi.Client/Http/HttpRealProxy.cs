using Lenic.Web.WebApi.Expressions;
using Lenic.Web.WebApi.HttpBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Lenic.Web.WebApi.Client.Http
{
    [DebuggerStepThrough]
    internal class HttpRealProxy<T> : RealProxy, IHttpProxy where T : class
    {
        #region Private Fields

        private Lazy<T> _proxy = null;

        #endregion Private Fields

        #region Business Properties

        public HttpClient ClientProxy { get; set; }

        public string BaseAddress { get; private set; }

        #endregion Business Properties

        #region Entrance

        public HttpRealProxy()
            : base(typeof(T))
        {
            BaseAddress = string.Format("{0}/{1}/", typeof(T).Namespace.Split('.').Last(), GetControllerName());

            _proxy = new Lazy<T>(() => (T)GetTransparentProxy(), true);
        }

        #endregion Entrance

        #region Business Methods

        public object BuildProxyInstance()
        {
            return GetTransparentProxy();
        }

        #endregion Business Methods

        #region Override Methods

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;

            if (methodCall.InArgCount != methodCall.ArgCount)
                throw new ArgumentException("远程不能包含 out 参数，请确认方法签名！");

            IMethodReturnMessage methodReturn = null;
            object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
            methodCall.Args.CopyTo(copiedArgs, 0);
            try
            {
                object returnValue = FetchData(methodCall);
                methodReturn = new ReturnMessage(returnValue, copiedArgs, copiedArgs.Length, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception ex)
            {
                methodReturn = new ReturnMessage(ex, methodCall);
            }

            return methodReturn;
        }

        #endregion Override Methods

        #region Private Methods

        private object FetchData(IMethodCallMessage methodCall)
        {
            var uri = BaseAddress + GetAction(methodCall.MethodBase);
            var relativeUri = ReplenishURL(methodCall);
            if (!string.IsNullOrWhiteSpace(relativeUri))
                uri = string.Format("{0}?{1}", uri, relativeUri);

            var absoluteUrl = string.Format("{0}#{1}", ClientProxy.BaseAddress, uri);
            var returnType = (methodCall.MethodBase as MethodInfo).ReturnType;

            var httpMethod = GetHttpMethod(methodCall.MethodBase);
            if (httpMethod == "GET")
            {
                return ClientProxy.GetAsync(uri).FetchValue(returnType, httpMethod, absoluteUrl);
            }
            else if (httpMethod == "DELETE")
                return ClientProxy.DeleteAsync(uri).FetchValue(returnType, httpMethod, absoluteUrl);
            else
            {
                var body = FetchBody(methodCall);

                if (httpMethod == "POST")
                    return ClientProxy.PostAsJsonAsync(uri, body).FetchValue(returnType, httpMethod, absoluteUrl);
                else if (httpMethod == "PUT")
                    return ClientProxy.PutAsJsonAsync(uri, body).FetchValue(returnType, httpMethod, absoluteUrl);

                throw new NotSupportedException("不支持的请求方法：" + httpMethod);
            }
        }

        private string GetHttpMethod(MethodBase method)
        {
            var methodName = method.GetCustomAttribute<HttpRequestAttribute>();
            if (methodName != null)
                return methodName.HttpMethod;

            if (method.Name.StartsWith("Get", StringComparison.CurrentCultureIgnoreCase))
                return "GET";
            else if (method.Name.StartsWith("Post", StringComparison.CurrentCultureIgnoreCase))
                return "POST";
            else if (method.Name.StartsWith("Put", StringComparison.CurrentCultureIgnoreCase))
                return "PUT";
            else if (method.Name.StartsWith("Delete", StringComparison.CurrentCultureIgnoreCase))
                return "DELETE";

            return "POST";
        }

        private string GetAction(MethodBase method)
        {
            var actionName = method.GetCustomAttribute<ActionNameAttribute>();
            if (actionName != null)
                return actionName.Value;

            return method.Name;
        }

        private string ReplenishURL(IMethodCallMessage methodCall)
        {
            var data = new List<string>();
            var parameters = methodCall.MethodBase.GetParameters();

            for (int i = 0; i < methodCall.ArgCount; i++)
            {
                var argName = methodCall.GetInArgName(i);
                var arg = methodCall.GetInArg(i);
                var parameter = parameters.First(p => p.Name == argName);

                var binding = parameter.GetCustomAttribute<ParameterBindingAttribute>();
                if (binding == null)
                    continue;

                if (binding.BindingType == "Uri")
                    data.Add(string.Format("{0}={1}", argName, Uri.EscapeUriString(ReferenceEquals(arg, null) ? "null" : arg.ToString())));
                else if (binding.BindingType == "ODataParameter" && !ReferenceEquals(arg, null))
                {
                    var dataParameter = new RemoteDataParameter();

                    var processor = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionProcessor>();
                    processor.Writer = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionWriter>();
                    processor.DataParameter = dataParameter;

                    processor.Build((arg as LambdaExpression).Body as MethodCallExpression);

                    data.Add(dataParameter.BuildUri());
                }
            }

            return string.Join("&", data);
        }

        private object FetchBody(IMethodCallMessage methodCall)
        {
            object body = null;
            var jsonBody = new Dictionary<string, object>();
            var parameters = methodCall.MethodBase.GetParameters();

            for (int i = 0; i < methodCall.ArgCount; i++)
            {
                var argName = methodCall.GetInArgName(i);
                var arg = methodCall.GetInArg(i);
                var parameter = parameters.First(p => p.Name == argName);

                var binding = parameter.GetCustomAttribute<ParameterBindingAttribute>();
                if (binding != null)
                {
                    if (binding.BindingType == "Body")
                    {
                        if (!ReferenceEquals(body, null))
                        {
                            var parameterName = string.Format("[{0}].[{1}]", typeof(T).Name, methodCall.MethodBase.Name);
                            throw new ArgumentException(parameterName, parameterName + " 有多个复杂类型传入，请更新全部参数使用 JsonBodyAttribute 参数绑定！");
                        }
                        body = arg;
                    }
                    else if (binding.BindingType == "JsonBody")
                        jsonBody.Add(argName, arg);
                }
                else
                    jsonBody.Add(argName, arg);
            }

            if (body != null && jsonBody.Count == 0)
                return body;
            else if (body == null && jsonBody.Count == 1)
                return jsonBody.First().Value;
            else if (body == null && jsonBody.Count != 0)
                return jsonBody;

            return null;
        }

        private string GetControllerName()
        {
            var data = typeof(T).Name.ToCharArray();
            List<string> result = new List<string>();
            List<char> current = new List<char>();

            foreach (var item in data)
            {
                if (char.IsLetter(item) && char.IsUpper(item) && current.Any())
                {
                    result.Add(new string(current.ToArray()));
                    current.Clear();
                }
                current.Add(item);
            }
            if (current.Any())
                result.Add(new string(current.ToArray()));

            if (result.Count < 3)
                throw new ArgumentException("控制器名称不符合规定！");

            return string.Join(string.Empty, result.Skip(1).Take(result.Count - 2));
        }

        #endregion Private Methods

        #region Hidden

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return base.CreateObjRef(requestedType);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override IntPtr GetCOMIUnknown(bool fIsMarshalled)
        {
            return base.GetCOMIUnknown(fIsMarshalled);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override object GetTransparentProxy()
        {
            return base.GetTransparentProxy();
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override void SetCOMIUnknown(IntPtr i)
        {
            base.SetCOMIUnknown(i);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override IntPtr SupportsInterface(ref Guid iid)
        {
            return base.SupportsInterface(ref iid);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        #endregion Hidden
    }
}