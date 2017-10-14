using System;
using System.Threading.Tasks;

namespace Lenic.Framework.Common.Threading
{
    /// <summary>
    /// 异步任务扩展方法集合
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 开始执行异步任务。
        /// </summary>
        /// <typeparam name="T">当前类型的类型。</typeparam>
        /// <param name="obj">当前实例对象。</param>
        /// <param name="action">需要执行的异步任务委托。</param>
        /// <returns>执行的异步任务信息。</returns>
        public static Task ExecuteAsync<T>(this T obj, Action<T> action)
        {
            return Task.Factory.StartNew(p =>
            {
                var parameter = p as Tuple<T, Action<T>>;

                parameter.Item2(parameter.Item1);
            }, Tuple.Create(obj, action));
        }
    }
}