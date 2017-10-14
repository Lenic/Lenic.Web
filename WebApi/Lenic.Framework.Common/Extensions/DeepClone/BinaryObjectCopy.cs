using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// 二进制类型对象克隆类
    /// </summary>
    public class BinaryObjectCopy : IObjectCopy
    {
        private IFormatter formatter = new BinaryFormatter();

        #region ICopyAction 成员

        /// <summary>
        /// 获取或设置待深层克隆的实例对象类型。
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// 从一个数据流中读取并生成一个 <see cref="ObjectType" /> 属性指定的新对象。
        /// </summary>
        /// <param name="stream">包含原始对象序列化数据的流。</param>
        /// <returns>一个新的对象，其数据从参数指定的流中获取。</returns>
        public object Deserialize(Stream stream)
        {
            return new BinaryFormatter().Deserialize(stream);
        }

        /// <summary>
        /// 将对象克隆生成一个流。
        /// </summary>
        /// <param name="obj">待克隆的原始对象，该对象不可为 <c>null</c> 。</param>
        /// <returns>包含原始对象序列化数据的流。</returns>
        public Stream Serialize(object obj)
        {
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, obj);

            return stream;
        }

        #endregion ICopyAction 成员
    }
}