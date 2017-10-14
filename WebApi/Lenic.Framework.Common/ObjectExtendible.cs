using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 可扩展的对象类
    /// </summary>
    /// <typeparam name="T">扩展的对象类型</typeparam>
    [DebuggerStepThrough]
    public class ObjectExtendible<T> : IObjectExtendible
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置扩展的对象信息。
        /// </summary>
        /// <value>扩展的对象信息。</value>
        public virtual T Object { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="ObjectExtendible{T}" /> 类的实例对象。
        /// </summary>
        public ObjectExtendible()
        {
            Tag = new Dictionary<string, object>();
        }

        /// <summary>
        /// 初始化新建一个 <see cref="ObjectExtendible{T}"/> 类的实例对象。
        /// </summary>
        /// <param name="obj">待扩展的原始对象。</param>
        public ObjectExtendible(T obj)
            : this()
        {
            Object = obj;
        }

        #endregion Entrance

        #region IObjectExtendible 成员

        /// <summary>
        /// 获取对象扩展属性。
        /// </summary>
        /// <value>对象扩展属性字典集合。</value>
        public virtual IDictionary<string, object> Tag { get; protected set; }

        #endregion IObjectExtendible 成员
    }
}