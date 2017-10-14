using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lenic.Framework.Common.Threading
{
    /// <summary>
    /// 串行任务类
    /// </summary>
    /// <typeparam name="T">串行任务需要处理的元素类型</typeparam>
    [DebuggerStepThrough]
    public sealed class SequenceTask<T>
    {
        #region Private Fields

        private ConcurrentStack<T> _cache;
        private Task<object> _defaultTask;
        private ReaderWriterLockSlim _locker;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 需要异步执行的逻辑委托。
        /// </summary>
        public Action<IEnumerable<T>> ActionFunction { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="SequenceTask{T}"/> 类的实例对象。
        /// </summary>
        public SequenceTask()
        {
            _cache = new ConcurrentStack<T>();
            _locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        #endregion Entrance

        #region Events

        /// <summary>
        /// 在异步任务发生错误的时候触发。
        /// </summary>
        public event Action<Exception> OnError;

        #endregion Events

        #region Business Methods

        /// <summary>
        /// 添加对象到串行处理任务。
        /// </summary>
        /// <param name="obj">待处理的任务对象。</param>
        public void AddObject(T obj)
        {
            _cache.Push(obj);

            StartTask();
        }

        #endregion Business Methods

        #region Private Methods

        private void StartTask()
        {
            if (_locker.TryEnterWriteLock(0))
            {
                if (ReferenceEquals(_defaultTask, null))
                    _defaultTask = Task.Factory.StartNew<object>(DoAction, Tuple.Create(ActionFunction, _cache));
                else
                {
                    switch (_defaultTask.Status)
                    {
                        case TaskStatus.Canceled:
                        case TaskStatus.Created:
                        case TaskStatus.RanToCompletion:
                            _defaultTask = Task.Factory.StartNew<object>(DoAction, Tuple.Create(ActionFunction, _cache));
                            break;

                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            _defaultTask = _defaultTask.ContinueWith<object>(DoActionAgain);
                            break;

                        case TaskStatus.Faulted:
                        default:
                            break;
                    }
                }
                _locker.ExitWriteLock();
            }
        }

        private object DoAction(object obj)
        {
            try
            {
                Execute(obj as Tuple<Action<IEnumerable<T>>, ConcurrentStack<T>>);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(ex);
            }

            return obj;
        }

        private object DoActionAgain(Task<object> obj)
        {
            try
            {
                Execute(obj.Result as Tuple<Action<IEnumerable<T>>, ConcurrentStack<T>>);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(ex);
            }

            return obj.Result;
        }

        private void Execute(Tuple<Action<IEnumerable<T>>, ConcurrentStack<T>> obj)
        {
            if (obj.Item2.Count != 0)
            {
                var data = new T[obj.Item2.Count];
                obj.Item2.TryPopRange(data, 0, data.Length);
                obj.Item1(data);
            }
        }

        #endregion Private Methods
    }
}