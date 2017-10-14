using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lenic.Framework.Common.Contexts;
using Lenic.Web.WebApi.Services.Pipelines.Request;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 数据上下文处理节点类
    /// </summary>
    public class DataContextHandler : IndexableHandler
    {
        /// <summary>
        /// 获取当前管道节点的处理次序索引：当前节点索引值为 double 类型最小值。
        /// </summary>
        public override double Index
        {
            get { return double.MinValue; }
        }

        /// <summary>
        /// 开启数据上下文，并异步执行后续 Http 管道处理请求。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <param name="cancellationToken">和异步执行取消操作有关的通知对象。</param>
        /// <returns>异步执行的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContextScope())
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}