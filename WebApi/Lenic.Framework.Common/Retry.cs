using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 重试方法类
    /// </summary>
    public class Retry
    {
        #region Business Properties

        /// <summary>
        /// 获取允许的重试次数。
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// 在执行过程中发生异常后触发。
        /// </summary>
        public Action<ExceptionContinueInfomation> AfterException { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 创建一个 <see cref="Retry"/> 类的实例对象。
        /// </summary>
        /// <param name="count">允许发生错误的次数：缺省为 3 次。</param>
        /// <returns>一个 <see cref="Retry"/> 类的实例对象。</returns>
        public static Retry Create(int count = 3)
        {
            return new Retry { RetryCount = count };
        }

        private Retry()
        {
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 在允许的重试次数内循环执行目标逻辑，直到执行成功。
        /// </summary>
        /// <param name="action">目标逻辑方法。</param>
        /// <returns>执行目标逻辑过程中发生的异常信息集合。</returns>
        public Exception[] Execute(Action action)
        {
            var dic = new Dictionary<int, Exception>();

            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    dic.Add(i, e);

                    if (AfterException != null)
                    {
                        try
                        {
                            var info = new ExceptionContinueInfomation(e);
                            AfterException(info);
                            if (!info.Continue)
                                break;
                        }
                        catch { }
                    }

                    continue;
                }

                break;
            }

            if (dic.Any() && dic.Count == RetryCount)
                throw new RetryException(dic.Values.ToArray());

            return dic.Values.ToArray();
        }

        #endregion Business Methods

        #region Inner Class

        /// <summary>
        /// 重试异常信息类
        /// </summary>
        public class RetryException : Exception
        {
            /// <summary>
            /// 执行目标逻辑过程中发生的异常信息集合。
            /// </summary>
            public IList<Exception> InnerExceptions { get; private set; }

            internal RetryException(IEnumerable<Exception> exceptions)
                : base("系统发生了重试异常！", exceptions == null ? null : exceptions.FirstOrDefault())
            {
                InnerExceptions = exceptions == null ? new List<Exception>() : exceptions.ToList();
            }
        }

        /// <summary>
        /// 异常重试信息
        /// </summary>
        public class ExceptionContinueInfomation
        {
            /// <summary>
            /// 获取发生的异常信息。
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            /// 获取或设置
            /// </summary>
            public bool Continue { get; set; }

            internal ExceptionContinueInfomation(Exception e)
            {
                Exception = e;
                Continue = true;
            }
        }

        #endregion Inner Class
    }

    /// <summary>
    /// 重试方法类扩展方法集合
    /// </summary>
    public static class _RetryExtensions
    {
        /// <summary>
        /// 在重试的过程中发生了异常后睡眠一段时间。
        /// </summary>
        /// <param name="retry">一个 <see cref="Retry"/> 类的实例对象。</param>
        /// <param name="timeout">睡眠的时长。</param>
        /// <returns>一个 <see cref="Retry"/> 类的实例对象。</returns>
        public static Retry SleepAfterException(this Retry retry, TimeSpan timeout)
        {
            retry.AfterException += p => Thread.Sleep(timeout);

            return retry;
        }
    }
}