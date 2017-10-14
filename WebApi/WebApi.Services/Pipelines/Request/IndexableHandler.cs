using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Lenic.Web.WebApi.Services.Pipelines.Request
{
    /// <summary>
    /// Http 请求管道处理节点
    /// </summary>
    public abstract class IndexableHandler : DelegatingHandler
    {
        /// <summary>
        /// 获取当前管道节点的处理次序索引。
        /// </summary>
        public abstract double Index { get; }

        /// <summary>
        /// 异步执行 Http 管道处理请求。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <param name="cancellationToken">和异步执行取消操作有关的通知对象。</param>
        /// <returns>异步执行的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}