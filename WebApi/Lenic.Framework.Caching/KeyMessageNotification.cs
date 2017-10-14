using System;
using Lenic.Framework.Common;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Logging;
using Lenic.Framework.Common.Messaging;

namespace Lenic.Framework.Caching
{
    /// <summary>
    /// 指定键消息缓存项通知类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class KeyMessageNotification<T> : CacheNotification
    {
        #region Business Properties

        /// <summary>
        /// 获取当前正在使用的消息路由。
        /// </summary>
        public IMessageRouter Router { get; private set; }

        /// <summary>
        /// 获取当前实例的唯一标识。
        /// </summary>
        public Guid Identity { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="KeyMessageNotification{T}"/> 类的实例对象。
        /// </summary>
        /// <param name="router">设置当前正在使用的消息路由实例对象。</param>
        public KeyMessageNotification(IMessageRouter router)
        {
            Identity = Guid.NewGuid();
            Router = router;

            Router.Register<ObjectExtendible<T>>(p =>
            {
                var obj = p.GetObject<ObjectExtendible<T>>();

                if (obj.GetChange() == ObjectChangeType.Update.ToString())
                    SetValue(obj.GetParameter<T>());
                else
                    SetValue(null);

                Notify();
            }, Identity, filter: p => string.Equals(p.Tag.GetValueX("Key", string.Empty), Key));
        }

        #endregion Entrance

        #region Override Methods

        /// <summary>
        /// 执行通知的核心方法：当前属性 Value 值为 <c>null</c> 时执行表示从消息路由中移除当前实例的订阅，否则不作任何操作。
        /// </summary>
        protected override void ExecuteNotifyCore()
        {
            if (ReferenceEquals(Value, null))
                Router.Unregister<ObjectExtendible<T>>(p => object.Equals(p.Identity, Identity));
        }

        #endregion Override Methods
    }
}