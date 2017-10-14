using Lenic.Framework.Common.Contexts;
using Lenic.Framework.Common.Exceptions;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Messaging;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// Http 客户端扩展方法集合
    /// </summary>
    public static class HttpClientExtensions
    {
        #region Public Properties

        /// <summary>
        /// 根据 Http 状态码获取响应信息处理逻辑委托：在未获取到时返回 <c>null</c>。
        /// </summary>
        public static Func<HttpStatusCode, IHttpStatusCodePolicy> GetHttpStatusCodePolicy { get; set; }

        #endregion Public Properties

        #region Direct Methods

        /// <summary>
        /// 通过 Http 方法获取目标值。
        /// </summary>
        /// <typeparam name="T">目标值的真实类型</typeparam>
        /// <param name="asyncRequest">一个异步的 Http 请求。</param>
        /// <param name="httpMethod">Http 请求的方法名称。</param>
        /// <param name="absoluteUrl">Http 请求的绝对路径。</param>
        /// <returns>符合条件的目标值实例对象。</returns>
        public static T FetchValue<T>(this Task<HttpResponseMessage> asyncRequest, string httpMethod, string absoluteUrl)
        {
            return (T)FetchValue(asyncRequest, typeof(T), httpMethod, absoluteUrl);
        }

        /// <summary>
        /// 通过 Http 方法获取目标值。
        /// </summary>
        /// <param name="asyncRequest">一个异步的 Http 请求。</param>
        /// <param name="targetType">请求的目标类型。</param>
        /// <param name="httpMethod">Http 请求的方法名称。</param>
        /// <param name="absoluteUrl">Http 请求的绝对路径。</param>
        /// <param name="policies"></param>
        /// <returns>符合条件的目标值实例对象。</returns>
        public static object FetchValue(this Task<HttpResponseMessage> asyncRequest, Type targetType, string httpMethod, string absoluteUrl)
        {
            Exception baseException = null;
            if (asyncRequest.Exception != null && (baseException = asyncRequest.Exception.GetBaseException()) != null)
                throw new BusinessException("请求【{2}】地址【{0}】返回类型【{1}】：请求过程出现异常！".With(absoluteUrl, targetType.FullName, httpMethod)).Setup(p =>
                {
                    p.Data.Add("URL", absoluteUrl);
                    p.Data.Add("Exception", baseException);
                });

            var response = asyncRequest.Result;

            if (!response.IsSuccessStatusCode && GetHttpStatusCodePolicy != null)
            {
                IHttpStatusCodePolicy policy = null;
                try
                {
                    policy = GetHttpStatusCodePolicy(response.StatusCode);
                }
                catch { }
                if (policy != null)
                    return policy.Execute(response);
            }

            if (!response.IsSuccessStatusCode)
                throw new BusinessException("请求【{2}】地址【{0}】返回类型【{1}】：返回结果出现异常！".With(absoluteUrl, targetType.FullName, httpMethod)).Setup(p =>
                {
                    p.Data.Add("URL", absoluteUrl);
                    p.Data.Add("StatusCode", response.StatusCode);
                    p.Data.Add("ReasonPhrase", response.ReasonPhrase);
                    p.Data.Add("ResponseString", response.Content.ReadAsStringAsync().Result);
                });

            if (targetType == typeof(void))
                return null;

            var resultString = string.Empty;
            try
            {
                try
                {
                    return response.Content.ReadAsAsync(targetType).Result;
                }
                catch
                {
                    return JsonConvert.DeserializeObject(resultString = response.Content.ReadAsStringAsync().Result, targetType);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("请求【{2}】地址【{0}】返回类型【{1}】：反序列化出现异常！".With(absoluteUrl, targetType.FullName, httpMethod), ex).Setup(p =>
                {
                    p.Data.Add("URL", absoluteUrl);
                    p.Data.Add("StatusCode", response.StatusCode);
                    p.Data.Add("ReasonPhrase", response.ReasonPhrase);
                    p.Data.Add("ResponseString", resultString ?? response.Content.ReadAsStringAsync().Result);
                });
            }
        }

        /// <summary>
        /// 通过 HttpDelete 方法获取目标值。
        /// </summary>
        /// <typeparam name="T">目标值的真实类型</typeparam>
        /// <param name="client">当前 Http 请求客户端的实例对象。</param>
        /// <param name="url">请求的目标地址。</param>
        /// <returns>符合条件的目标值实例对象。</returns>
        public static T FetchValueByDelete<T>(this HttpClient client, string url)
        {
            var absoluteUrl = string.Concat(client.BaseAddress, "#", url);

            return FetchValue<T>(client.DeleteAsync(url), "DELETE", absoluteUrl);
        }

        /// <summary>
        /// 通过 HttpGet 方法获取目标值。
        /// </summary>
        /// <typeparam name="T">目标值的真实类型</typeparam>
        /// <param name="client">当前 Http 请求客户端的实例对象。</param>
        /// <param name="url">请求的目标地址。</param>
        /// <returns>符合条件的目标值实例对象。</returns>
        public static T FetchValueByGet<T>(this HttpClient client, string url)
        {
            var absoluteUrl = string.Concat(client.BaseAddress, "#", url);

            return FetchValue<T>(client.GetAsync(url), "GET", absoluteUrl);
        }

        /// <summary>
        /// 通过 HttpPost 方法获取目标值。
        /// </summary>
        /// <typeparam name="T">目标值的真实类型</typeparam>
        /// <param name="client">当前 Http 请求客户端的实例对象。</param>
        /// <param name="body">请求需要的内容信息。</param>
        /// <param name="url">请求的目标地址。</param>
        /// <returns></returns>
        public static T FetchValueByPost<T>(this HttpClient client, object body, string url)
        {
            var absoluteUrl = string.Concat(client.BaseAddress, "#", url);

            return FetchValue<T>(client.PostAsync(url, body, new JsonMediaTypeFormatter()), "POST", absoluteUrl);
        }

        /// <summary>
        /// 通过 HttpPut 方法获取目标值。
        /// </summary>
        /// <typeparam name="T">目标值的真实类型</typeparam>
        /// <param name="client">当前 Http 请求客户端的实例对象。</param>
        /// <param name="body">请求需要的内容信息。</param>
        /// <param name="url">请求的目标地址。</param>
        /// <returns></returns>
        public static T FetchValueByPut<T>(this HttpClient client, object body, string url)
        {
            var absoluteUrl = string.Concat(client.BaseAddress, "#", url);

            return FetchValue<T>(client.PutAsync(url, body, new JsonMediaTypeFormatter()), "PUT", absoluteUrl);
        }

        #endregion Direct Methods

        #region HttpClient Methods

        /// <summary>
        /// 添加缺省请求头信息，在未获取到值的情况下，不添加到请求头信息中。
        /// </summary>
        /// <param name="client">一个 <see cref="HttpClient"/> 类的实例对象。</param>
        /// <param name="name">要添加的 Http 请求头的名称。</param>
        /// <param name="defaultValue">">要添加的 Http 请求头的缺省值：值从 DataContext.Current 对象实例中获取。</param>
        public static void AddHead(this HttpClient client, string name, string defaultValue = null)
        {
            object value = null;
            if (DataContext.Current != null)
                value = DataContext.Current.Items.GetValueX(name, defaultValue);
            else
                value = defaultValue;

            if (value != null)
                client.DefaultRequestHeaders.Add(name, Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString())));
        }

        #endregion HttpClient Methods
    }
}