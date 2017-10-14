using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using Newtonsoft.Json;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// Json 字符串格式数值绑定
    /// </summary>
    public class JsonParameterBinding : HttpParameterBinding
    {
        /// <summary>
        /// 初始化新建一个 <see cref="JsonParameterBinding"/> 类的实例对象。
        /// </summary>
        /// <param name="desc">绑定参数的描述性信息。</param>
        public JsonParameterBinding(HttpParameterDescriptor desc)
            : base(desc)
        {
        }

        /// <summary>
        /// 以异步方式执行给定请求的绑定。
        /// </summary>
        /// <param name="metadataProvider">要用于验证的元数据提供程序。</param>
        /// <param name="actionContext">绑定的操作上下文。操作上下文包含将使用参数填充的参数字典。</param>
        /// <param name="cancellationToken">用于取消绑定操作的取消标记。</param>
        /// <returns>一个表示异步操作的任务对象。</returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // read the object value to real type
            object value = actionContext.Request.RequestUri.ParseQueryString().GetValues(Descriptor.ParameterName)[0];
            if (value != null)
                value = JsonConvert.DeserializeObject((string)value, Descriptor.ParameterType);

            // Set the binding result here
            SetValue(actionContext, value);

            // now, we can return a completed task with no result
            TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }

        /// <summary>
        /// 无返回异步结果
        /// </summary>
        private struct AsyncVoid
        {
        }
    }
}