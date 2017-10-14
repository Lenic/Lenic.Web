namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// Content-Type 序列化接口
    /// </summary>
    public interface IContentTypeSerializer
    {
        /// <summary>
        /// 获取当前实例对象的 Content-Type 。
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// 序列化指定的对象为字节数组。
        /// </summary>
        /// <param name="obj">需要序列化的实例对象。</param>
        /// <returns>
        /// 序列化完成的字节数组。
        /// </returns>
        byte[] Serialize(object obj);
    }
}