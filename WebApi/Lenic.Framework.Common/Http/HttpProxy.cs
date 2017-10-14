using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// Http 客户端代理类
    /// </summary>
    [DebuggerStepThrough]
    public class HttpProxy
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置 User-Agent Http 标头的值，默认值为：Intime.Windows.Common HttpProxy
        /// </summary>
        public static string UserAgent = "Intime.Windows.Common HttpProxy";

        /// <summary>
        /// 获取或设置 Accept HTTP 标头的值。
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 获取请求数据过程中用到的所有 Cookie 信息。
        /// </summary>
        /// <value>
        /// 请求数据过程中用到的所有 Cookie 信息。
        /// </value>
        public CookieContainer Cookies { get; private set; }

        /// <summary>
        /// 获取请求相关的头信息集合。
        /// </summary>
        /// <value>
        /// 请求相关的头信息集合。
        /// </value>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// 获取或设置请求过程中用到的 Web 代理实例对象。
        /// </summary>
        /// <value>
        /// 请求过程中用到的 Web 代理实例对象。
        /// </value>
        public WebProxy Proxy { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="HttpProxy"/> 类的实例对象。
        /// </summary>
        public HttpProxy()
        {
            Cookies = new CookieContainer();
            Headers = new Dictionary<string, string>();
        }

        #endregion Entrance

        #region Methods

        /// <summary>
        /// 创建并开始异步请求数据。
        /// </summary>
        /// <param name="contentType">设置 Content-Type Http 标头的值。</param>
        /// <param name="method">设置请求的方法。</param>
        /// <param name="url">标识 Internet 资源的 URI。</param>
        /// <param name="body">请求的内容体。</param>
        /// <returns>
        /// 异步操作状态的结果。
        /// </returns>
        /// <exception cref="System.ArgumentException">Content type is missing</exception>
        /// <exception cref="System.ArgumentException">Content type is missing</exception>
        /// <exception cref="System.ArgumentException">Content type is missing</exception>
        public IAsyncResult StartRequest(string contentType, string method, string url, byte[] body)
        {
            if (body == null)
                body = new byte[0];

            if (IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type is missing");

            if (IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL is empty");

            if (method != "GET" && method != "POST" && method != "DELETE" && method != "PUT")
                throw new ArgumentException("Invalid Method");

            try
            {
                var client = (HttpWebRequest)WebRequest.Create(new Uri(url));
                client.CookieContainer = Cookies;
                client.Method = method;
                client.Accept = Accept;
                client.ContentType = contentType;

                foreach (var item in Headers)
                    client.Headers.Add(item.Key, item.Value);

                client.UserAgent = UserAgent;

                client.Proxy = Proxy;

                if (method == "POST" || method == "PUT" || method == "DELETE")
                {
                    return client.BeginGetRequestStream(callbackResult =>
                    {
                        try
                        {
                            var request = (HttpWebRequest)callbackResult.AsyncState;

                            // Write to the client stream.
                            using (Stream postStream = request.EndGetRequestStream(callbackResult))
                            {
                                postStream.Write(body, 0, body.Length);
                                postStream.Flush();
                            }

                            // Start the asynchronous operation to get the response
                            request.BeginGetResponse(ProcessCallback, request);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                if (OnFailureCompleted != null)
                                    OnFailureCompleted(ex);
                            }
                            finally
                            {
                                if (OnAllCompleted != null)
                                    OnAllCompleted();
                            }
                        }
                    }, client);
                }
                if (method == "GET")
                {
                    return client.BeginGetResponse(ProcessCallback, client);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (OnFailureCompleted != null)
                        OnFailureCompleted(ex);
                }
                finally
                {
                    if (OnAllCompleted != null)
                        OnAllCompleted();
                }
            }
            return null;
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// 在其它事件全部完成之后，触发。
        /// </summary>
        public event Action OnAllCompleted;

        /// <summary>
        /// 在接收服务端数据发生错误时，触发。
        /// </summary>
        public event Action<Exception> OnFailureCompleted;

        /// <summary>
        /// 在成功完成接收服务端数据的时候，触发。
        /// </summary>
        public event Action<ResponseInfo> OnSuccessCompleted;

        #endregion Events

        #region Private Methods

        private bool IsNullOrWhiteSpace(string obj)
        {
            var result = string.IsNullOrEmpty(obj);

            return result || obj.All(char.IsWhiteSpace);
        }

        private void ProcessCallback(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            try
            {
                using (var response = (HttpWebResponse)request.EndGetResponse(result))
                {
                    ProcessSuccessCompleted(response, response.StatusDescription);
                }
            }
            catch (WebException ex)
            {
                try
                {
                    ProcessSuccessCompleted((HttpWebResponse)ex.Response, ex.Message);
                }
                catch (Exception e)
                {
                    if (OnFailureCompleted != null)
                        OnFailureCompleted(e);
                }
            }
            catch (Exception ex)
            {
                if (OnFailureCompleted != null)
                    OnFailureCompleted(ex);
            }
            finally
            {
                if (OnAllCompleted != null)
                    OnAllCompleted();
            }
        }

        private void ProcessSuccessCompleted(HttpWebResponse response, string description)
        {
            var info = new ResponseInfo
            {
                StatusCode = response.StatusCode,
                StatusDescription = description,
                Headers = response.Headers
            };

            if (OnSuccessCompleted != null)
            {
                using (info.ResponseStream = new MemoryStream())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        int currentChar;

                        while ((currentChar = stream.ReadByte()) != -1)
                            info.ResponseStream.WriteByte((byte)currentChar);
                    }
                    info.ResponseStream.Seek(0, SeekOrigin.Begin);

                    OnSuccessCompleted(info);
                }
            }
        }

        #endregion Private Methods
    }
}