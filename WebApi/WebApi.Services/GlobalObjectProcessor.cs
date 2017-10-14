using System;
using System.Collections.Generic;

namespace Lenic.Web.WebApi.Services
{
    /// <summary>
    /// 全局异常处理器
    /// </summary>
    /// <param name="e">待处理的异常信息。</param>
    /// <param name="controllerType">发生异常的控制器类型。</param>
    /// <param name="obj">承载处理结果的对象，用于返回给调用方。</param>
    public delegate void ExceptionProcessor(Exception e, Type controllerType, ref object obj);

    /// <summary>
    /// 高级的全局异常处理器
    /// </summary>
    /// <param name="e">待处理的异常信息。</param>
    /// <param name="controllerType">发生异常的控制器类型。</param>
    /// <param name="parameters">处理过程中用到的参数列表。</param>
    /// <param name="obj">承载处理结果的对象，用于返回给调用方。</param>
    public delegate void AdvancedExceptionProcessor(Exception e, Type controllerType, GlobalObjectParameters parameters, ref object obj);

    /// <summary>
    /// 全局实例对象处理器
    /// </summary>
    /// <param name="controllerType">待处理的控制器类型。</param>
    /// <param name="returnType">待处理的方法返回值类型。</param>
    /// <param name="obj">承载处理结果的对象，用于返回给调用方。</param>
    public delegate void ObjectProcessor(Type controllerType, Type returnType, ref object obj);

    /// <summary>
    /// 高级的全局实例对象处理器
    /// </summary>
    /// <param name="controllerType">待处理的控制器类型。</param>
    /// <param name="returnType">待处理的方法返回值类型。</param>
    /// <param name="parameters">处理过程中用到的参数列表。</param>
    /// <param name="obj">承载处理结果的对象，用于返回给调用方。</param>
    public delegate void AdvancedObjectProcessor(Type controllerType, Type returnType, GlobalObjectParameters parameters, ref object obj);

    /// <summary>
    /// 全局实例对象处理器
    /// </summary>
    public abstract class GlobalObjectProcessor
    {
        /// <summary>
        /// 请求处理过程中发生异常，在完成后返回前，做最后的处理工作时触发（在 OnGlobalException2 事件之后处理）。
        /// </summary>
        public static event ExceptionProcessor OnGlobalException;

        /// <summary>
        /// 请求处理过程中发生异常，在完成后返回前，做最后的处理工作时触发（包含高级参数）。
        /// </summary>
        public static event AdvancedExceptionProcessor OnGlobalException2;

        /// <summary>
        /// 请求处理过程中未发生异常，在完成后返回前，做最后的处理工作时触发（在 OnGlobalCompleted2 事件之后处理）。
        /// </summary>
        public static event ObjectProcessor OnGlobalCompleted;

        /// <summary>
        /// 请求处理过程中未发生异常，在完成后返回前，做最后的处理工作时触发（包含高级参数）。
        /// </summary>
        public static event AdvancedObjectProcessor OnGlobalCompleted2;

        /// <summary>
        /// 初始化新建一个 <see cref="GlobalObjectProcessor"/> 类的实例对象。
        /// </summary>
        protected GlobalObjectProcessor()
        {
        }

        /// <summary>
        /// 引发全局异常处理事件。
        /// </summary>
        /// <param name="e">待处理的异常信息。</param>
        /// <param name="controllerType">发生异常的控制器类型。</param>
        /// <returns>处理完成后的对象。</returns>
        public virtual object RaiseExceptionEvent(Exception e, Type controllerType, Func<GlobalObjectParameters> parameters)
        {
            object obj = null;
            if (OnGlobalException2 != null)
                OnGlobalException2(e, controllerType, parameters(), ref obj);
            if (OnGlobalException != null)
                OnGlobalException(e, controllerType, ref obj);

            return obj;
        }

        /// <summary>
        /// 引发对象完成的处理事件。
        /// </summary>
        /// <param name="obj">待处理的目标对象信息。</param>
        /// <param name="controllerType">待处理的控制器类型。</param>
        /// <param name="returnType">待处理的方法返回值类型。</param>
        /// <param name="tag">处理事件的额外参数字典。</param>
        /// <returns>处理完成后的对象。</returns>
        public virtual object RaiseCompletedEvent(object obj, Type controllerType, Type returnType, Func<GlobalObjectParameters> parameters)
        {
            if (OnGlobalCompleted2 != null)
                OnGlobalCompleted2(controllerType, returnType, parameters(), ref obj);
            if (OnGlobalCompleted != null)
                OnGlobalCompleted(controllerType, returnType, ref obj);

            return obj;
        }
    }
}