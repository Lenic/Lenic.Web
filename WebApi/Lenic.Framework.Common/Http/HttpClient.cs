using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Lenic.Framework.Common.Extensions;

namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// Http 客户端同步代理类
    /// </summary>
    public sealed class HttpClient
    {
        #region Private Fields

        private string _userAgent = "Intime.Framework.Common HttpClient";

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取或设置 User-Agent Http 标头的值，默认值为：Intime.Framework.Common HttpProxy
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// 获取或设置请求的 URL 基址。
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// 获取或设置 Accept HTTP 标头的值。
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 获取请求数据过程中用到的所有 Cookie 信息。
        /// </summary>
        /// <value>请求数据过程中用到的所有 Cookie 信息。</value>
        public CookieContainer Cookies { get; private set; }

        /// <summary>
        /// 获取请求相关的头信息集合。
        /// </summary>
        /// <value>请求相关的头信息集合。</value>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// 获取或设置请求过程中用到的 Web 代理实例对象。
        /// </summary>
        /// <value>请求过程中用到的 Web 代理实例对象。</value>
        public WebProxy Proxy { get; set; }

        /// <summary>
        /// 获取或设置全局请求设置信息。
        /// </summary>
        public HttpClientSetting Setting { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="HttpClient"/> 类的实例对象。
        /// </summary>
        public HttpClient()
        {
            Cookies = new CookieContainer();
            Headers = new Dictionary<string, string>();
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 创建并开始异步请求数据。
        /// </summary>
        /// <param name="contentType">设置 Content-Type Http 标头的值。</param>
        /// <param name="method">设置请求的方法。</param>
        /// <param name="url">标识 Internet 资源的 URI。</param>
        /// <param name="body">请求的内容体。</param>
        /// <param name="action">调用成功的回调函数。</param>
        /// <exception cref="System.ArgumentNullException">[HttpClient].[Request].contentType or
        /// [HttpClient].[Request].url or [HttpClient].[Request].method</exception>
        public void Request(string contentType, string method, string url, byte[] body, Action<ResponseInfo> action)
        {
            if (body == null)
                body = new byte[0];

            if (contentType.IsNullOrWhiteSpace())
                throw new ArgumentNullException("[HttpClient].[Request].contentType");

            if (url.IsNullOrWhiteSpace())
                throw new ArgumentNullException("[HttpClient].[Request].url");
            else if (!BaseDirectory.IsNullOrWhiteSpace())
            {
                var hasAppendBase = BaseDirectory.EndsWith("//");
                var hasInsertUrl = url.StartsWith("//");
                if (!(hasAppendBase || hasInsertUrl))
                    url = string.Concat(BaseDirectory, "//", url);
                else
                    url = string.Concat(BaseDirectory, url);
            }

            if (method.IsNullOrWhiteSpace())
                throw new ArgumentNullException("[HttpClient].[Request].method");
            else
                method = method.ToUpper();

            if (method != "GET" && method != "POST" && method != "DELETE" && method != "PUT")
                throw new ArgumentException("[HttpClient].[Request].method is invalid method.");

            var client = (HttpWebRequest)WebRequest.Create(new Uri(url));
            client.CookieContainer = Cookies;
            client.Method = method;
            client.Accept = Accept;
            client.ContentType = contentType;
            client.UserAgent = UserAgent;
            client.Proxy = Proxy;

            if (Setting != null)
            {
                client.AutomaticDecompression = Setting.DecompressionMethod;
                client.Credentials = Setting.Credentials;
            }

            foreach (var item in Headers)
                client.Headers.Add(item.Key, item.Value);

            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                using (var stream = client.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Flush();
                }
            }

            using (var response = GetResponseObject(client.GetResponse() as HttpWebResponse))
            {
                action(response);
            }
        }

        #endregion Business Methods

        #region Private Methods

        private ResponseInfo GetResponseObject(HttpWebResponse response)
        {
            var info = new ResponseInfo
            {
                StatusCode = response.StatusCode,
                Headers = response.Headers
            };
            info.ResponseStream = new MemoryStream();
            response.GetResponseStream().CopyTo(info.ResponseStream);
            info.ResponseStream.Seek(0, SeekOrigin.Begin);

            return info;
        }

        #endregion Private Methods
    }
}