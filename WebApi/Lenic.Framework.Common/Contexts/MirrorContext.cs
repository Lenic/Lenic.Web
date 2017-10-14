using System;
using System.Collections.Generic;
using System.Threading;

namespace Lenic.Framework.Common.Contexts
{
    /// <summary>
    /// 数据上下文镜像
    /// </summary>
    [Serializable]
    public class MirrorContext : DataContext
    {
        #region Business Properties

        /// <summary>
        /// 获取保存镜像时，系统的线程信息
        /// </summary>
        public Thread OriginalThread { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="MirrorContext"/> 类的实例对象
        /// </summary>
        /// <param name="context">一个共享数据上下文的实例对象</param>
        public MirrorContext(DataContext context)
        {
            OriginalThread = Thread.CurrentThread;
            this.Items = new Dictionary<string, object>(context.Items);
        }

        #endregion Entrance
    }
}