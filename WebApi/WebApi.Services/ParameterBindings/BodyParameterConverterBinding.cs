using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// Json 字符串格式数值绑定
    /// </summary>
    public class BodyParameterConverterBinding : HttpParameterBinding
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
            get { return false; }
        }

        /// <summary>
        /// 获取转换类类型。
        /// </summary>
        public Type ConverterType { get; internal set; }

        /// <summary>
        /// 获取转换类的转换方法名称。
        /// </summary>
        public string MethodName { get; internal set; }

        internal Tuple<Type, string> CacheKey { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="BodyParameterConverterBinding"/> 类的实例对象。
        /// </summary>
        /// <param name="desc">绑定参数的描述性信息。</param>
        public BodyParameterConverterBinding(HttpParameterDescriptor desc)
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
            string content = actionContext.Request.Content.ReadAsStringAsync().Result;

            try
            {
                var value = GetFunction()(content);
                SetValue(actionContext, value);
            }
            catch (Exception ex)
            {
                var e = ex;
                while (e.InnerException != null)
                    e = e.InnerException;

                _errorMessage = e.Message;
            }

            var tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }

        #endregion Override Members

        #region Private Methods

        private Func<string, object> GetFunction()
        {
            return _execFuncCache.GetOrAdd(CacheKey, key =>
            {
                var methodInfo = key.Item1.GetMethod(key.Item2, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                if (methodInfo == null)
                    throw new ModuleConfigException(string.Format("在类[{0}]中没有发现方法[{1}]。", key.Item1.FullName, key.Item2));

                var p = Expression.Parameter(typeof(string), "p");
                var call = Expression.Call(methodInfo, p);
                var c = Expression.Convert(call, typeof(object));

                var lambda = Expression.Lambda<Func<string, object>>(c, p);

                return lambda.Compile();
            });
        }

        #endregion Private Methods

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