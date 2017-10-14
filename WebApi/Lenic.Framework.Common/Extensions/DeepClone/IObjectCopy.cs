using System;
using System.IO;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// 对象深层克隆接口
    /// </summary>
    public interface IObjectCopy
    {
        /// <summary>
        /// 获取或设置待深层克隆的实例对象类型。
        /// </summary>
        Type ObjectType { get; set; }

        /// <summary>
        /// 将对象克隆生成一个流。
        /// </summary>
        /// <param name="obj">待克隆的原始对象，该对象不可为 <c>null</c> 。</param>
        /// <returns>包含原始对象序列化数据的流。</returns>
        Stream Serialize(object obj);

        /// <summary>
        /// 从一个数据流中读取并生成一个 <see cref="ObjectType"/> 属性指定的新对象。
        /// </summary>
        /// <param name="stream">包含原始对象序列化数据的流。</param>
        /// <returns>一个新的对象，其数据从参数指定的流中获取。</returns>
        object Deserialize(Stream stream);
    }
}