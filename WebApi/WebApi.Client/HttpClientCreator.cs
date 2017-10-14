using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Security;
using Lenic.Web.WebApi.Client.Properties;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// Http 客户端代理创建者类
    /// </summary>
    public class HttpClientCreator
    {
        #region Private Fields

        private static readonly RsaHelper Encoder = new RsaHelper(Resources.RasKey);
        private HttpClientHandler _handler = null;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取或设置服务端请求基址。
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// 获取客户端身份令牌。
        /// </summary>
        public object Identity { get; private set; }

        /// <summary>
        /// 获取或设置原始登录密文。
        /// </summary>
        protected string LoginCiphertext { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="HttpClientCreator"/> 类的实例对象。
        /// </summary>
        /// <param name="identity">经过验证的授权信息。</param>
        public HttpClientCreator(object identity)
        {
            Identity = identity;

            _handler = new HttpClientHandler();
            _handler.AutomaticDecompression = DecompressionMethods.GZip;
            _handler.UseProxy = false;
        }

        /// <summary>
        /// 向服务端注册，并获取一个 Http 客户端代理创建者类的实例对象。
        /// </summary>
        /// <param name="baseAddress">服务端的基址 URL 。</param>
        /// <param name="name">The name.</param>
        /// <param name="code">The code.</param>
        /// <param name="clientCode">The client code.</param>
        /// <returns>一个 Http 客户端代理创建者类的实例对象。</returns>
        /// <exception cref="System.Net.WebException">未能获取到身份令牌！</exception>
        public static HttpClientCreator RegistCurrentClient(string baseAddress, string name, string code, string clientCode)
        {
            var ciphertext = string.Format("\"{0}\"", Encoder.Encrypt(JsonConvert.SerializeObject(new
            {
                Name = name,
                Code = code,
                Client = clientCode,
            })));
            var content = new StringContent(ciphertext, Encoding.UTF8, "application/json");

            var result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                var response = client.PostAsync("Authorization/UserToken/RegistToken", content).Result;
                response.EnsureSuccessStatusCode();

                result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(result))
                    throw new WebException("未能获取到身份令牌！");
            }

            return new HttpClientCreator(new ValidatedInfomation
            {
                Name = name,
                Code = result,
                Client = clientCode,
            })
            {
                BaseAddress = baseAddress,
                LoginCiphertext = ciphertext,
            };
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 克隆一个定向到 <paramref name="baseAddress"/> 的创建者类实例对象。
        /// </summary>
        /// <param name="baseAddress">创建者类定义到的目标网址基址。</param>
        /// <returns>一个 Http 请求的客户端代理。</returns>
        public virtual HttpClientCreator Clone(string baseAddress)
        {
            var content = new StringContent(LoginCiphertext, Encoding.UTF8, "application/json");

            var result = string.Empty;
            using (var client = new HttpClient() { BaseAddress = new Uri(baseAddress) })
            {
                var response = client.PostAsync("Authorization/UserToken/RegistToken", content).Result;
                response.EnsureSuccessStatusCode();

                result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(result))
                    throw new WebException("未能获取到身份令牌！");
            }

            var info = Identity as ValidatedInfomation;
            info.Code = result;

            return new HttpClientCreator(info)
            {
                BaseAddress = baseAddress,
                LoginCiphertext = LoginCiphertext,
            };
        }

        /// <summary>
        /// 创建一个 Http 请求的客户端代理。
        /// </summary>
        /// <param name="baseAddress">客户端代理的基地址（比如：http://www.yintai.com:8080/）。</param>
        /// <returns>一个 Http 请求的客户端代理。</returns>
        public virtual HttpClient CreateClient(string baseAddress = null)
        {
            var client = new HttpClient(_handler);
            if (baseAddress != null)
            {
                if (baseAddress.IndexOf(BaseAddress, StringComparison.CurrentCultureIgnoreCase) == 0)
                    client.BaseAddress = new Uri(baseAddress);
                else
                    throw new ArgumentException("基地址只能在一个网站内变动，并且比原基地址长！", "[HttpClientCreator].[CreateClient].baseAddress").Setup(p =>
                    {
                        p.Data.Add("OriginalBaseAddress", BaseAddress);
                        p.Data.Add("NewBaseAddress", baseAddress);
                    });
            }
            else
                client.BaseAddress = new Uri(BaseAddress);

            if (!ReferenceEquals(Identity, null))
            {
                var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(Encoder.Encrypt(JsonConvert.SerializeObject(Identity))));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Intime", value);
            }

            return client;
        }

        /// <summary>
        /// 用于登录信息过期后重新登录。
        /// </summary>
        public virtual void Relogin()
        {
            var content = new StringContent(LoginCiphertext, Encoding.UTF8, "application/json");

            var result = string.Empty;
            using (var client = new HttpClient() { BaseAddress = new Uri(BaseAddress) })
            {
                var response = client.PostAsync("Authorization/UserToken/RegistToken", content).Result;
                response.EnsureSuccessStatusCode();

                result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(result))
                    throw new WebException("未能获取到身份令牌！");
            }

            var info = Identity as ValidatedInfomation;
            info.Code = result;
        }

        #endregion Business Methods

        #region Inner Class

        /// <summary>
        /// 已经过验证的登录信息类
        /// </summary>
        protected class ValidatedInfomation
        {
            /// <summary>
            /// 获取或设置登录人员的来源客户端。
            /// </summary>
            public string Client { get; set; }

            /// <summary>
            /// 获取或设置登录人员的编码。
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// 获取或设置登录人员的名称。
            /// </summary>
            public string Name { get; set; }
        }

        #endregion Inner Class
    }
}