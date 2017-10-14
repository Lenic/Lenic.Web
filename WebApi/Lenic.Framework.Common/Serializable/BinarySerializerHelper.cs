using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lenic.Framework.Common.Serializable
{
    /// <summary>
    /// * 二进制序列化辅助类
    /// </summary>
    public class BinarySerializerHelper
    {
        #region 二进制序列化

        /// <summary>
        /// * 反序列化二进制数据
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <param name="binFile">文件</param>
        /// <returns></returns>
        public static TObject BinDeserializeObject<TObject>(string binFile) where TObject : class
        {
            using (FileStream fs = new FileStream(binFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return BinDeserializeObject<TObject>(fs);
            }
        }

        /// <summary>
        /// * 反序列化二进制数据
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <param name="stream">序列化流</param>
        /// <returns></returns>
        public static TObject BinDeserializeObject<TObject>(Stream stream) where TObject : class
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf
                = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (TObject)bf.Deserialize(stream);
        }

        /// <summary>
        /// * 二进制序列化对象为文件
        /// </summary>
        /// <param name="binFile">文件</param>
        /// <param name="obj">源对象</param>
        public static void BinSerializeObject(string binFile, object obj)
        {
            using (FileStream fs = new FileStream(binFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinSerializeObject(fs, obj);
            }
        }

        /// <summary>
        /// * 二进制序列化对象
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        public static void BinSerializeObject(Stream stream, object obj)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf
                = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            bf.Serialize(stream, obj);
        }

        /// <summary>
        /// * 二进制序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] BinSerializeObject(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf
                    = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(stream, obj);

                return stream.ToArray();
            }
        }

        #endregion 二进制序列化
    }
}