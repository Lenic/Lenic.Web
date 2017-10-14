using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Lenic.Framework.Common.IO
{
    /// <summary>
    /// 文件监视类
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("FilePath = {FilePath}")]
    public sealed class FileWatcher
    {
        #region Private Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FileSystemWatcher fsw = null;

        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取一个值，通过该值指示是否可以引发文件内容改变事件：<c>true</c> 表示可以引发改变事件，默认值为 <c>true</c> 。
        /// </summary>
        public bool CanRaiseChangedEvent { get; set; }

        /// <summary>
        /// 获取监视文件的绝对路径.
        /// </summary>
        public string FilePath { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="FileWatcher" /> 类的实例对象.
        /// </summary>
        /// <param name="filePath">待监视文件的绝对路径.</param>
        /// <exception cref="System.IO.FileNotFoundException">没有找到待监视的文件！</exception>
        public FileWatcher(string filePath)
            : this()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("没有找到待监视的文件！", filePath);

            FilePath = filePath;
            InitFileSystemWatcher();
            CanRaiseChangedEvent = true;
        }

        /// <summary>
        /// 初始化新建一个 <see cref="FileWatcher" /> 类的实例对象.
        /// </summary>
        public FileWatcher()
        {
        }

        /// <summary>
        /// 创建一个 <see cref="FileWatcher" /> 类的实例对象.
        /// </summary>
        /// <param name="filePath">待监视文件的绝对路径.</param>
        /// <returns>创建完成的 <see cref="FileWatcher" /> 类实例对象.</returns>
        public static FileWatcher CreateOne(string filePath)
        {
            return new FileWatcher(filePath);
        }

        #endregion Entrance

        #region Events

        /// <summary>
        /// 在原始 XML 文件内容发生改变的时候发生
        /// </summary>
        public event Action OnXmlFileChanged;

        /// <summary>
        /// 引发 OnXmlFileChanged 事件
        /// </summary>
        public void RaiseChangedEvent()
        {
            if (OnXmlFileChanged != null && CanRaiseChangedEvent)
            {
                OnXmlFileChanged();
            }
        }

        #endregion Events

        #region Private Methods

        private void InitFileSystemWatcher()
        {
            fsw = new FileSystemWatcher(Path.GetDirectoryName(FilePath), Path.GetFileName(FilePath))
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.Security,
            };

            fsw.Changed += (sender, e) =>
            {
                var work = sender as FileSystemWatcher;

                if (_locker.TryEnterWriteLock(0))
                {
                    try
                    {
                        switch (e.ChangeType)
                        {
                            case WatcherChangeTypes.Changed:
                                work.EnableRaisingEvents = false;
                                RaiseChangedEvent();
                                work.EnableRaisingEvents = true;
                                break;

                            case WatcherChangeTypes.Deleted:
                            case WatcherChangeTypes.Renamed:
                                work.EnableRaisingEvents = false;
                                break;

                            case WatcherChangeTypes.All:
                            case WatcherChangeTypes.Created:
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            };
            fsw.EnableRaisingEvents = true;
        }

        #endregion Private Methods
    }
}