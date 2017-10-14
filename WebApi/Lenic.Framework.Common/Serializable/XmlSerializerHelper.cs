using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Lenic.Framework.Common.Serializable
{
    /// <summary>
    /// XML序列化帮助类
    /// </summary>
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// serialize object to xml string
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>XML string</returns>
        public static string GetXmlStringFromObject<T>(T obj)
        {
            using (var ms = new MemoryStream())
            using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                writer.Indentation = 3;
                writer.IndentChar = ' ';
                writer.Formatting = Formatting.Indented;
                var sc = new XmlSerializer(obj.GetType());
                sc.Serialize(writer, obj);
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                int index = xml.IndexOf("?>");
                if (index > 0)
                    xml = xml.Substring(index + 2);
                return xml.Trim();
            }
        }

        /// <summary>
        /// Deserialize object from xml file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">xml file name</param>
        /// <returns></returns>
        public static T LoadObjectFromXml<T>(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Deserialize object from xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">xml string</param>
        /// <returns></returns>
        public static T LoadObjectFromXmlString<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (StringReader r = new StringReader(xml)) 
            {
                return (T)serializer.Deserialize(r);
            }
            #region * 使用StringReader代替MemoryStream
            //using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            //{
            //    return (T)serializer.Deserialize(ms);
            //}
            #endregion
        }

        /// <summary>
        /// * 将指定XSD对象序列化到指定XML文件
        /// </summary>
        /// <param name="xmlFile">目标XML文件</param>
        /// <param name="schemaObject">要序列化的对象</param>
        public static void SerializeSchemaObjectToFile(string xmlFile, object schemaObject)
        {
            SerializeSchemaObjectToFile(xmlFile, schemaObject, null);
        }

        /// <summary>
        /// * 以指定文本编码将指定XSD对象序列化到指定XML文件
        /// </summary>
        /// <param name="xmlFile">目标XML文件</param>
        /// <param name="schemaObject">要序列化的对象</param>
        /// <param name="encoding">文本编码</param>
        public static void SerializeSchemaObjectToFile(string xmlFile, object schemaObject, Encoding encoding)
        {
            using (FileStream fs = new FileStream(xmlFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                XmlSerializer xs = new XmlSerializer(schemaObject.GetType());

                if (encoding == null)
                {
                    xs.Serialize(fs, schemaObject);
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(fs, encoding))
                    {
                        xs.Serialize(writer, schemaObject);
                    }
                }
            }
        }

        /// <summary>
        /// * 将指定XSD对象序列化为字符串
        /// </summary>
        /// <param name="schemaObject">要序列化的对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeSchemaObjectToString(object schemaObject)
        {
            StringBuilder builder = new StringBuilder();

            XmlSerializer xs = new XmlSerializer(schemaObject.GetType());

            using (TextWriter writer = new StringWriter(builder))
            {
                xs.Serialize(writer, schemaObject);

                writer.Flush();
            }

            return builder.ToString();
        }

        /// <summary>
        /// copy a instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Copy<T>(T t)
        {
            string xml = GetXmlStringFromObject(t);
            return LoadObjectFromXmlString<T>(xml);
        }
    }
}