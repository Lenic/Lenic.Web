using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using Newtonsoft.Json.Linq;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// Json 字符串格式数值绑定
    /// </summary>
    public class JsonBodyParameterBinding : HttpParameterBinding
    {
        #region Private Fields

        private string _errorMessage = null;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 如果绑定无效，则获取描述绑定错误的错误消息。
        /// </summary>
        /// <returns>错误消息。如果绑定成功，则此值为 null。</returns>
        public override string ErrorMessage
        {
            get { return _errorMessage; }
        }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="JsonBodyParameterBinding"/> 类的实例对象。
        /// </summary>
        /// <param name="desc">绑定参数的描述性信息。</param>
        public JsonBodyParameterBinding(HttpParameterDescriptor desc)
            : base(desc)
        {
        }

        #endregion Entrance

        #region Override Members

        /// <summary>
        /// 以异步方式执行给定请求的绑定。
        /// </summary>
        /// <param name="metadataProvider">要用于验证的元数据提供程序。</param>
        /// <param name="actionContext">绑定的操作上下文。操作上下文包含将使用参数填充的参数字典。</param>
        /// <param name="cancellationToken">用于取消绑定操作的取消标记。</param>
        /// <returns>一个表示异步操作的任务对象。</returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // init json parameter cache
            object jsonParameters = null;
            if (!actionContext.ActionArguments.TryGetValue("__#JsonParameters#__", out jsonParameters))
                jsonParameters = new ConcurrentDictionary<string, JToken>(StringComparer.CurrentCultureIgnoreCase);
            var parameterCache = jsonParameters as ConcurrentDictionary<string, JToken>;

            // read the object value to real type
            var obj = parameterCache.GetOrAdd(Descriptor.ParameterName, p =>
            {
                JToken result = null;
                string value = actionContext.Request.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(value))
                    _errorMessage = "[JsonBodyParameterBinding].[ExecuteBindingAsync].actionContext.Request.Content == null";
                else
                {
                    var data = JObject.Parse(value);
                    foreach (var item in data.Properties())
                        parameterCache.AddOrUpdate(item.Name, item.Value, (key, originalValue) => item.Value);

                    parameterCache.TryGetValue(p, out result);
                }
                return result;
            });

            // Set the binding result here
            if (IsValid)
                SetValue(actionContext, (obj == null ? Descriptor.DefaultValue : obj.ToObject(Descriptor.ParameterType)));

            // now, we can return a completed task with no result
            TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }

        #endregion Override Members

        #region Private Struct: AsyncVoid

        /// <summary>
        /// 无返回异步结果
        /// </summary>
        private struct AsyncVoid
        {
        }

        #endregion Private Struct: AsyncVoid
    }
}