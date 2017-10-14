using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// 异常信息扩展方法集合
    /// </summary>
    [DebuggerStepThrough]
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取异常类中内含的所有异常实例对象。
        /// </summary>
        /// <param name="obj">一个异常类的实例对象。</param>
        /// <returns>
        /// 当前异常实例中内含的所有异常实例对象。
        /// </returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static IEnumerable<Exception> GetExceptions(this Exception obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var data = new List<Exception>();

            var work = obj;
            do
            {
                data.Add(work);
                work = work.InnerException;
            } while (work != null);

            return data;
        }
    }
}