using System.Net;

namespace Lenic.Framework.Common.Http
{
    /// <summary>
    /// Http 请求设置信息
    /// </summary>
    public class HttpClientSetting
    {
        /// <summary>
        /// 获取缺省的 Http 请求设置信息。
        /// </summary>
        public static readonly HttpClientSetting DefaultInstance = new HttpClientSetting();

        /// <summary>
        /// 获取或设置压缩信息。
        /// </summary>
        public DecompressionMethods DecompressionMethod { get; set; }

        /// <summary>
        /// 获取或设置身份验证信息。
        /// </summary>
        public ICredentials Credentials { get; set; }
    }
}