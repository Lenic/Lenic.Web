using System;
using Lenic.Framework.Common.Http.Serializers;

namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// Http 客户端扩展方法集合
    /// </summary>
    public static class RequestExtensions
    {
        #region GET POST PUT DELETE

        /// <summary>
        /// 发送一个 HTTP DELETE 异步资源请求。
        /// </summary>
        /// <param name="client">一个 Http 客户端的实例对象。</param>
        /// <param name="url">请求资源的目标 URL 。</param>
        /// <param name="serializer">Content-Type 指定的序列化方式。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        public static IAsyncResult DeleteAsync(this HttpProxy client, string url, IContentTypeSerializer serializer = null)
        {
            return SendAsync(client, "DELETE", url, null, serializer);
        }

        /// <summary>
        /// 发送一个 HTTP GET 异步资源请求。
        /// </summary>
        /// <param name="client">一个 Http 客户端的实例对象。</param>
        /// <param name="url">请求资源的目标 URL 。</param>
        /// <param name="serializer">Content-Type 指定的序列化方式。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        public static IAsyncResult GetAsync(this HttpProxy client, string url, IContentTypeSerializer serializer = null)
        {
            return SendAsync(client, "GET", url, null, serializer);
        }

        /// <summary>
        /// 发送一个 HTTP POST 异步资源请求。
        /// </summary>
        /// <param name="client">一个 Http 客户端的实例对象。</param>
        /// <param name="url">请求资源的目标 URL 。</param>
        /// <param name="parameter">请求资源的过程中用到的参数。</param>
        /// <param name="serializer">Content-Type 指定的序列化方式。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        public static IAsyncResult PostAsync(this HttpProxy client, string url, object parameter = null, IContentTypeSerializer serializer = null)
        {
            return SendAsync(client, "POST", url, parameter, serializer);
        }

        /// <summary>
        /// 发送一个 HTTP PUT 异步资源请求。
        /// </summary>
        /// <param name="client">一个 Http 客户端的实例对象。</param>
        /// <param name="url">请求资源的目标 URL 。</param>
        /// <param name="parameter">请求资源的过程中用到的参数。</param>
        /// <param name="serializer">Content-Type 指定的序列化方式。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        public static IAsyncResult PutAsync(this HttpProxy client, string url, object parameter = null, IContentTypeSerializer serializer = null)
        {
            return SendAsync(client, "PUT", url, parameter, serializer);
        }

        #endregion GET POST PUT DELETE

        #region Send Async

        /// <summary>
        /// 发送一个 HTTP 异步资源请求。
        /// </summary>
        /// <param name="client">一个 Http 客户端的实例对象。</param>
        /// <param name="httpMethod">请求的 HTTP 方法。</param>
        /// <param name="url">请求资源的目标 URL 。</param>
        /// <param name="parameter">请求资源的过程中用到的参数。</param>
        /// <param name="serializer">Content-Type 指定的序列化方式。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        public static IAsyncResult SendAsync(this HttpProxy client, string httpMethod, string url, object parameter, IContentTypeSerializer serializer = null)
        {
            if (serializer == null)
                serializer = new FormContentTypeSerializer();

            return client.StartRequest(serializer.ContentType, httpMethod.ToUpper(), url, serializer.Serialize(parameter));
        }

        #endregion Send Async
    }
}