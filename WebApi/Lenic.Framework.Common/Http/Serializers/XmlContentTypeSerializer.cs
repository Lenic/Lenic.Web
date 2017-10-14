using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Lenic.Framework.Common.Http.Serializers
{
    /// <summary>
    /// Xml 格式 Content-Type 序列化类 -- {DataContractSerializer}
    /// </summary>
    public class XmlContentTypeSerializer : IContentTypeSerializer
    {
        #region IContentTypeSerializer 成员

        /// <summary>
        /// 获取当前实例对象的 Content-Type 。
        /// </summary>
        public string ContentType
        {
            get { return "application/xml"; }
        }

        /// <summary>
        /// 序列化指定的对象为字节数组。
        /// </summary>
        /// <param name="obj">需要序列化的实例对象。</param>
        /// <returns>
        /// 序列化完成的字节数组。
        /// </returns>
        public byte[] Serialize(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return new byte[0];

            if (obj is string)
                return Encoding.UTF8.GetBytes((string)obj);

            using (var ms = new MemoryStream())
            {
                new DataContractSerializer(obj.GetType()).WriteObject(ms, obj);

                return ms.ToArray();
            }
        }

        #endregion IContentTypeSerializer 成员
    }
}