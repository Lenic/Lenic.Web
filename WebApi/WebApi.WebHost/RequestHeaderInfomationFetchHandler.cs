using Lenic.Framework.Common.Contexts;
using Lenic.Web.WebApi.Services.Pipelines.Request;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lenic.Web.WebApi.WebHost
{
    /// <summary>
    /// 授权信息获取处理器
    /// </summary>
    public class RequestHeaderInfomationFetchHandler : IndexableHandler
    {
        private static readonly double _defaultIndex = double.MinValue + 2;

        /// <summary>
        /// 获取当前管道节点的处理次序索引：当前节点索引值为 double 类型最小值 + 2。
        /// </summary>
        public override double Index
        {
            get { return _defaultIndex; }
        }

        /// <summary>
        /// 异步执行 Http 管道处理请求，添加请求的自定义头到数据上下文中。
        /// </summary>
        /// <param name="request">一个 Http 请求的实例对象。</param>
        /// <param name="cancellationToken">和异步执行取消操作有关的通知对象。</param>
        /// <returns>异步执行的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddItem(request, "UserId");
            AddItem(request, "LoginAccount");
            AddItem(request, "UserName");
            AddItem(request, "DeptId");
            AddItem(request, "IP");
            AddItem(request, "MachineName");
            AddItem(request, "WarehouseId");
            AddItem(request, "WarehouseCode");
            AddItem(request, "WarehouseName");
            AddItem(request, "LargeVersion");

            return base.SendAsync(request, cancellationToken);
        }

        private void AddItem(HttpRequestMessage request, string name)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues(name, out values))
            {
                var value = values.FirstOrDefault();
                if (value != null)
                    DataContext.Current.SetValue(name, Encoding.UTF8.GetString(Convert.FromBase64String(value)));
            }
        }
    }
}