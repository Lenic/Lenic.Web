using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Xml.Serialization;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// Json 字符串格式数值绑定
    /// </summary>
    public class XmlParameterBinding : HttpParameterBinding
    {
        #region Private Fields

        private static ConcurrentDictionary<Tuple<Type, string>, Func<string, object>> _execFuncCache =
                    new ConcurrentDictionary<Tuple<Type, string>, Func<string, object>>(Environment.ProcessorCount * 2, 16);

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

        /// <summary>
        /// 返回一个值，通过该值指示 <see cref="T:System.Web.Http.Controllers.HttpParameterBinding" /> 是否将要读取
        /// Body 内容。
        /// </summary>
        /// <returns><c>true</c> 表示
        /// <see cref="T:System.Web.Http.Controllers.HttpParameterBinding" /> 已经读取了 Body 内容；否则返回
        /// <c>false</c> 。</returns>
        public override bool WillReadBody
        {
            get { return true; }
        }

        /// <summary>
        /// 获取目标类类型。
        /// </summary>
        public Type TargetType { get; internal set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="XmlParameterBinding"/> 类的实例对象。
        /// </summary>
        /// <param name="desc">绑定参数的描述性信息。</param>
        public XmlParameterBinding(HttpParameterDescriptor desc)
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
            var length = actionContext.Request.Content.Headers.ContentLength;
            if (!length.HasValue || length.Value == 0L)
                SetValue(actionContext, Descriptor.DefaultValue);
            else
                try
                {
                    var value = new XmlSerializer(TargetType ?? Descriptor.ParameterType).Deserialize(actionContext.Request.Content.ReadAsStreamAsync().Result);
                    SetValue(actionContext, value);
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.ToString();
                }

            var tcs = new TaskCompletionSource<AsyncVoid>();
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