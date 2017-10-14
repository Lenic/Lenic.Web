using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lenic.Framework.Common.Http.Serializers
{
    /// <summary>
    /// Form 格式 Content-Type 序列化类
    /// </summary>
    public class FormContentTypeSerializer : IContentTypeSerializer
    {
        /// <summary>
        /// 获取当前实例对象的 Content-Type 。
        /// </summary>
        public string ContentType
        {
            get { return "application/x-www-form-urlencoded"; }
        }

        /// <summary>
        /// 序列化指定的对象为字节数组。
        /// </summary>
        /// <param name="obj">需要序列化的实例对象。</param>
        /// <returns>
        /// 序列化完成的字节数组。
        /// </returns>
        /// <exception cref="System.ArgumentNullException">paramter cannot be a null object</exception>
        public byte[] Serialize(object obj)
        {
            if (ReferenceEquals(obj, null))
                return new byte[0];

            var s = obj as string;
            if (s != null)
                return Encoding.UTF8.GetBytes(s);

            var data = new List<string>();
            try
            {
                data.AddRange(from property in obj.GetType().GetProperties() 
                              let value = Uri.EscapeDataString(property.GetValue(obj, null).ToString()) 
                              select string.Format("{0}={1}", property.Name, value));
            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("paramter cannot be a null object", e);
            }

            return Encoding.UTF8.GetBytes(string.Join("&", data.ToArray()));
        }
    }
}