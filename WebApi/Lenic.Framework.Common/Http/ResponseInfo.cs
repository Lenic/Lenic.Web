using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// 响应信息类
    /// </summary>
    [DebuggerStepThrough]
    public class ResponseInfo : IDisposable
    {
        #region Private Fields

        private string _body;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取响应信息关联的标头。
        /// </summary>
        public WebHeaderCollection Headers { get; internal set; }

        /// <summary>
        /// 获取一个值，通过该值指示响应是否成功：<c>true</c> 表示是一个成功的响应。
        /// </summary>
        public bool IsSuccessStatusCode
        {
            get { return ((StatusCode >= HttpStatusCode.OK) && (StatusCode <= ((HttpStatusCode)0x12b))); }
        }

        /// <summary>
        /// 获取 HTTP 响应的 Body 信息。
        /// </summary>
        public string ResponseBody
        {
            get
            {
                if (_body == null)
                {
                    lock (this)
                    {
                        if (_body != null)
                            return _body;

                        var index = ResponseStream.Position;
                        if (index != 0)
                            ResponseStream.Seek(0, SeekOrigin.Begin);

                        Interlocked.Exchange(ref _body, new StreamReader(ResponseStream).ReadToEnd());

                        ResponseStream.Seek(index, SeekOrigin.Begin);
                    }
                }

                return _body;
            }
        }

        /// <summary>
        /// 获取原始响应流。
        /// </summary>
        public Stream ResponseStream { get; internal set; }

        /// <summary>
        /// 获取 HTTP 状态代码。
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// 获取 HTTP 状态代码相关的描述性信息。
        /// </summary>
        public string StatusDescription { get; internal set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="ResponseInfo"/> 类的实例对象。
        /// </summary>
        internal ResponseInfo()
        {
        }

        #endregion Entrance

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            if (ResponseStream != null)
                ResponseStream.Close();
        }

        #endregion IDisposable 成员
    }
}