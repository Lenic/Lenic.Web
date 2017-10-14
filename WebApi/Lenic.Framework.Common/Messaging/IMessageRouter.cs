using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lenic.Framework.Common.Messaging
{
    /// <summary>
    /// 消息路由接口
    /// </summary>
    public interface IMessageRouter
    {
        /// <summary>
        /// 在执行发布方法发生异常时触发。
        /// </summary>
        RaisePublishException OnPublishException { get; set; }

        /// <summary>
        /// 获取或设置全局发布配置。
        /// </summary>
        IList<ISubscriber> GlobalPublish { get; }

        /// <summary>
        /// 获取发布容器。
        /// </summary>
        ConcurrentDictionary<string, IList<ISubscriber>> Store { get; }

        /// <summary>
        /// 向各个订阅的客户端发布消息。
        /// </summary>
        /// <param name="type">发布消息的类型。</param>
        /// <param name="msg">待发布的消息。</param>
        /// <returns>发布管道的执行上下文。</returns>
        EventContext Publish(string type, object msg);
    }

    /// <summary>
    /// 消息路由接口扩展方法集合
    /// </summary>
    public static class IMessageRouterExtensions
    {
        /// <summary>
        /// 初始化 <paramref name="obj"/> 的发布异常处理。
        /// </summary>
        /// <param name="obj">一个 <see cref="IMessageRouter"/> 类的实例对象。</param>
        public static void PublishExceptionInitialization(IMessageRouter obj)
        {
            if (ReferenceEquals(obj.OnPublishException, null))
                obj.OnPublishException += delegate(object sender, PublishExceptionArgs e)
                {
                    var router = sender as IMessageRouter;
                    e.Exception.Data["Object"] = e.Object;
                    router.Publish(e.Exception);
                };
        }

        /// <summary>
        /// 向各个订阅的客户端发布消息。
        /// </summary>
        /// <typeparam name="TMessage">待发送消息的类型。</typeparam>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="msg">待发送的消息实例对象。</param>
        /// <returns>发布管道的执行上下文。</returns>
        public static EventContext Publish<TMessage>(this IMessageRouter obj, TMessage msg)
        {
            return obj.Publish(typeof(TMessage).AssemblyQualifiedName, msg);
        }

        /// <summary>
        /// 异步向各个订阅的客户端发布消息。
        /// </summary>
        /// <typeparam name="TMessage">待发送消息的类型。</typeparam>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="msg">待发送的消息实例对象。</param>
        /// <returns>一个表示异步消息状态的实例对象。</returns>
        public static IAsyncResult PublishAsync<TMessage>(this IMessageRouter obj, TMessage msg)
        {
            Func<IMessageRouter, TMessage, EventContext> func = Publish;

            return func.BeginInvoke(obj, msg, null, null);
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <typeparam name="TMessage">客户端订阅的消息类型</typeparam>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static IMessageRouter Register<TMessage>(this IMessageRouter obj, Action<EventContext> action, object identity = null, double index = 0, Func<TMessage, bool> filter = null)
        {
            Func<object, bool> subscriberFilter = p => p is TMessage;
            if (filter != null)
                subscriberFilter = p => p is TMessage && filter((TMessage)p);

            var subscriber = new Subscriber
            {
                Action = action,
                Filter = subscriberFilter,
                Identity = identity,
                Index = index,
            };
            obj.Store.GetOrAdd(typeof(TMessage).AssemblyQualifiedName, p => new List<ISubscriber>()).Add(subscriber);

            return obj;
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="key">注册的发布键。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static IMessageRouter Register(this IMessageRouter obj, string key, Action<EventContext> action, object identity = null, double index = 0, Func<object, bool> filter = null)
        {
            var subscriber = new Subscriber
            {
                Action = action,
                Filter = filter,
                Identity = identity,
                Index = index,
            };
            obj.Store.GetOrAdd(key, p => new List<ISubscriber>()).Add(subscriber);

            return obj;
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <typeparam name="TMessage">客户端订阅的消息类型</typeparam>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static AsyncSubscriber RegisterAsync<TMessage>(this IMessageRouter obj, Action<object> action, object identity = null, double index = 0, Func<TMessage, bool> filter = null)
        {
            Func<object, bool> subscriberFilter = p => p is TMessage;
            if (filter != null)
                subscriberFilter = p => p is TMessage && filter((TMessage)p);

            var subscriber = new AsyncSubscriber
            {
                Action = action,
                Filter = subscriberFilter,
                Identity = identity,
                Index = index,
            };
            obj.Store.GetOrAdd(typeof(TMessage).AssemblyQualifiedName, p => new List<ISubscriber>()).Add(subscriber);

            return subscriber;
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="key">注册的发布键。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static AsyncSubscriber RegisterAsync(this IMessageRouter obj, string key, Action<object> action, object identity = null, double index = 0, Func<object, bool> filter = null)
        {
            var subscriber = new AsyncSubscriber
            {
                Action = action,
                Filter = filter,
                Identity = identity,
                Index = index,
            };
            obj.Store.GetOrAdd(key, p => new List<ISubscriber>()).Add(subscriber);

            return subscriber;
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static IMessageRouter RegisterGlobal(this IMessageRouter obj, Action<EventContext> action, object identity = null, double index = 0, Func<object, bool> filter = null)
        {
            var subscriber = new Subscriber
            {
                Action = action,
                Filter = filter,
                Identity = identity,
                Index = index,
            };
            obj.GlobalPublish.Add(subscriber);

            return obj;
        }

        /// <summary>
        /// 向消息事件缓存接口中注册新的客户端订阅。
        /// </summary>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="action">订阅的客户端消息处理逻辑。</param>
        /// <param name="identity">当前处理逻辑的标识：可以多个逻辑使用同一个标识。</param>
        /// <param name="index">消息在处理队列中的顺序：升序处理。</param>
        /// <param name="filter">服务器消息过滤器.</param>
        /// <returns>已经完成注册的消息路由扩展接口实例对象。</returns>
        public static AsyncSubscriber RegisterGlobalAsync(this IMessageRouter obj, Action<object> action, object identity = null, double index = 0, Func<object, bool> filter = null)
        {
            var subscriber = new AsyncSubscriber
            {
                Action = action,
                Filter = filter,
                Identity = identity,
                Index = index,
            };
            obj.GlobalPublish.Add(subscriber);

            return subscriber;
        }

        /// <summary>
        /// 解除客户端在消息事件缓存接口中的注册。
        /// </summary>
        /// <typeparam name="TMessage">客户端订阅的消息类型</typeparam>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="predicate">待删除逻辑的委托方法。</param>
        /// <returns>已经完成卸载的消息路由扩展接口实例对象。</returns>
        public static IMessageRouter Unregister<TMessage>(this IMessageRouter obj, Func<ISubscriber, bool> predicate = null)
        {
            IList<ISubscriber> result = null;
            var key = typeof(TMessage).AssemblyQualifiedName;
            if (predicate == null)
                obj.Store.TryRemove(key, out result);
            else
            {
                var data = obj.Store[key];
                foreach (var item in data.ToArray())
                {
                    if (predicate(item))
                        data.Remove(item);
                }

                if (data.Count == 0)
                    obj.Store.TryRemove(key, out result);
            }

            return obj;
        }

        /// <summary>
        /// 解除客户端在消息事件缓存接口中的注册。
        /// </summary>
        /// <param name="obj">一个消息路由扩展接口的实例对象。</param>
        /// <param name="key">注册的发布键。</param>
        /// <param name="predicate">待删除逻辑的委托方法。</param>
        /// <returns>已经完成卸载的消息路由扩展接口实例对象。</returns>
        public static IMessageRouter Unregister(this IMessageRouter obj, string key, Func<ISubscriber, bool> predicate = null)
        {
            IList<ISubscriber> result = null;
            if (predicate == null)
                obj.Store.TryRemove(key, out result);
            else
            {
                var data = obj.Store[key];
                foreach (var item in data.ToArray())
                {
                    if (predicate(item))
                        data.Remove(item);
                }

                if (data.Count == 0)
                    obj.Store.TryRemove(key, out result);
            }

            return obj;
        }
    }
}