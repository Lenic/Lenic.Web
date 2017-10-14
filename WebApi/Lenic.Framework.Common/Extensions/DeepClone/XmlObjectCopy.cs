using System;
using System.IO;
using System.Xml.Serialization;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// Xml 形式对象克隆类
    /// </summary>
    public class XmlObjectCopy : IObjectCopy
    {
        private XmlSerializer _serializer = null;

        /// <summary>
        /// 获取 XML 序列化实例对象。
        /// </summary>
        public XmlSerializer Serializer
        {
            get { return (_serializer ?? (_serializer = new XmlSerializer(ObjectType))); }
        }

        #region IObjectCopy 成员

        /// <summary>
        /// 获取或设置待深层克隆的实例对象类型。
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// 将对象克隆生成一个流。
        /// </summary>
        /// <param name="obj">待克隆的原始对象，该对象不可为 <c>null</c> 。</param>
        /// <returns>包含原始对象序列化数据的流。</returns>
        public Stream Serialize(object obj)
        {
            Stream stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            return stream;
        }

        /// <summary>
        /// 从一个数据流中读取并生成一个 <see cref="ObjectType" /> 属性指定的新对象。
        /// </summary>
        /// <param name="stream">包含原始对象序列化数据的流。</param>
        /// <returns>一个新的对象，其数据从参数指定的流中获取。</returns>
        public object Deserialize(Stream stream)
        {
            return Serializer.Deserialize(stream);
        }

        #endregion IObjectCopy 成员
    }
}