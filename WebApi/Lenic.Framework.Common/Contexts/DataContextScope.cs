using System;
using System.Threading;

namespace Lenic.Framework.Common.Contexts
{
    /// <summary>
    /// 共享数据上下文范围
    /// </summary>
    public class DataContextScope : IDisposable
    {
        #region Private Fields

        private DataContext _originalContext = DataContext.Current;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="DataContextScope"/> 类的实例对象
        /// </summary>
        /// <param name="contextOption">共享数据上下文的配置信息</param>
        public DataContextScope(DataContextOption contextOption = DataContextOption.Required)
        {
            switch (contextOption)
            {
                case DataContextOption.RequiresNew:
                    DataContext.Current = new DataContext();
                    break;

                case DataContextOption.Required:
                    DataContext.Current = _originalContext ?? new DataContext();
                    break;

                case DataContextOption.Suppress:
                    DataContext.Current = null;
                    break;
            }
        }

        /// <summary>
        /// 初始化新建一个 <see cref="DataContextScope"/> 类的实例对象
        /// </summary>
        /// <param name="mirrorContext">一个共享数据上下文的某时刻镜像</param>
        public DataContextScope(MirrorContext mirrorContext)
        {
            if (mirrorContext.OriginalThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException("The DependentContextScope cannot be created in the thread in which the DependentContext is created.");
            }
            DataContext.Current = mirrorContext;
        }

        #endregion Entrance

        #region IDisposable 成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务
        /// </summary>
        public void Dispose()
        {
            DataContext.Current = _originalContext;
        }

        #endregion IDisposable 成员
    }
}