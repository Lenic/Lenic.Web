using System.Collections.Generic;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 可扩展对象接口
    /// </summary>
    public interface IObjectExtendible
    {
        /// <summary>
        /// 获取对象扩展属性。
        /// </summary>
        /// <value>
        /// 对象扩展属性字典集合。
        /// </value>
        IDictionary<string, object> Tag { get; }
    }
}