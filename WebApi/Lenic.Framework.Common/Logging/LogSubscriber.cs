using System;
using System.Diagnostics;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 日志订阅者类
    /// </summary>
    [DebuggerStepThrough]
    public abstract class LogSubscriber
    {
        #region Business Properties

        /// <summary>
        /// 获取订阅者实例对象。
        /// </summary>
        public Subscriber Subscriber { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="LogSubscriber"/> 类的实例对象。
        /// </summary>
        public LogSubscriber()
        {
            Subscriber = new Subscriber
            {
                Action = DoWork,
                Filter = p => p is LogEntry,
                Identity = Guid.NewGuid(),
            };
        }

        #endregion Entrance

        #region Override Methods

        /// <summary>
        /// 执行真正的处理工作。
        /// </summary>
        /// <param name="context">订阅事件上下文实例对象。</param>
        protected abstract void DoWork(EventContext context);

        #endregion Override Methods
    }
}