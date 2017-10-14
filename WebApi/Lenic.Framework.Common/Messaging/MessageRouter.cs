using Lenic.Framework.Common.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 消息路由类
    /// </summary>
    public class MessageRouter : IMessageRouter
    {
        #region Entrance

        /// <summary>
        /// 初始化新建一个消息路由类的实例对象。
        /// </summary>
        public MessageRouter()
        {
            Store = new ConcurrentDictionary<string, IList<ISubscriber>>();
            GlobalPublish = new List<ISubscriber>();
        }

        #endregion Entrance

        #region IMessageRouter 成员

        /// <summary>
        /// 在执行发布方法发生异常时触发。
        /// </summary>
        public RaisePublishException OnPublishException { get; set; }

        /// <summary>
        /// 获取或设置全局发布配置。
        /// </summary>
        public IList<ISubscriber> GlobalPublish { get; private set; }

        /// <summary>
        /// 获取发布容器。
        /// </summary>
        public ConcurrentDictionary<string, IList<ISubscriber>> Store { get; private set; }

        /// <summary>
        /// 向各个订阅的客户端发布消息。
        /// </summary>
        /// <param name="type">发布消息的类型。</param>
        /// <param name="msg">待发布的消息。</param>
        /// <returns>发布管道的执行上下文。</returns>
        public EventContext Publish(string type, object msg)
        {
            var context = new EventContext(msg);

            IList<ISubscriber> result = null;
            if (Store.TryGetValue(type, out result))
            {
                if (GlobalPublish != null && GlobalPublish.Any())
                    result = result.Concat(GlobalPublish)
                                   .Distinct(new EqualComparer<ISubscriber>((x, y) => object.ReferenceEquals(x, y)))
                                   .OrderBy(p => p.Index)
                                   .ToList();
                else
                    result = result.ToList();
            }
            else
                result = GlobalPublish.ToList();

            PublishCore(result, context);
            return context;
        }

        #endregion IMessageRouter 成员

        #region Private Methods

        private void PublishCore(IList<ISubscriber> data, EventContext context)
        {
            if (data == null && data.Count == 0)
                return;

            foreach (var item in data)
            {
                if (context.IsStopped)
                    break;

                if (item.Filter == null || item.Filter(context.Object))
                {
                    if (item is AsyncSubscriber)
                    {
                        var subscriber = item as AsyncSubscriber;

                        subscriber.AsyncHander = item.Action.BeginInvoke(context.Object, asyncResult =>
                        {
                            var result = (AsyncResult)asyncResult;

                            var action = (Action<object>)result.AsyncDelegate;

                            try
                            {
                                action.EndInvoke(result);
                            }
                            catch (Exception e)
                            {
                                RaiseException(result.AsyncState, e);
                            }
                        }, context.Object);
                    }
                    else
                    {
                        try
                        {
                            item.Action(context);
                        }
                        catch (Exception e)
                        {
                            RaiseException(context.Object, e);

                            if (context.Exception == null)
                                context.Exception = new AggregateException(e);
                            else if (context.Exception is AggregateException)
                            {
                                var ex = context.Exception as AggregateException;

                                var exceptions = ex.InnerExceptions.ToList();
                                exceptions.Add(e);

                                context.Exception = new AggregateException(exceptions);
                            }
                            else
                                context.Exception = new AggregateException(context.Exception, e);
                        }
                    }
                }
            }
        }

        private void RaiseException(object obj, Exception e)
        {
            if (OnPublishException != null)
            {
                AggregateException ex = e is AggregateException ? e as AggregateException : new AggregateException(e);
                try
                {
                    OnPublishException(this, new PublishExceptionArgs(obj, ex));
                }
                catch { }
            }
        }

        #endregion Private Methods
    }
}