using Lenic.Web.WebApi.Expressions;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// Http 协议远程对象获取器
    /// </summary>
    public class HttpRemoteDataFetcher : IRemoteDataFetcher
    {
        /// <summary>
        /// 获取或设置 Http 请求客户端代理。
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// 获取一个全新的远程对象获取器。
        /// </summary>
        /// <param name="client">Http 客户端请求代理。</param>
        /// <returns>创建完成的一个 Http 协议远程对象获取器实例对象。</returns>
        public static IRemoteDataFetcher GetInstance(HttpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client", "[HttpRemoteDataFetcher].[GetInstance].client is null reference.");

            return new HttpRemoteDataFetcher
            {
                Client = client
            };
        }

        /// <summary>
        /// 从远程获取对象实例。
        /// </summary>
        /// <param name="parameter">获取前检索用的参数信息。</param>
        /// <param name="targetType">需要获取的最终类型。</param>
        /// <returns>符合条件的实例对象。</returns>
        public object GetObject(RemoteDataParameter parameter, Type targetType)
        {
            var uri = parameter.BuildUri();

            var method = "GetByOData";
            if (parameter.Executor == "Count" || parameter.Executor == "LongCount")
                method = "CountByOData";
            else if (parameter.Executor == "Any")
                method = "AnyByOData";

            var absoluteUrl = string.Format("{0}#{1}", Client.BaseAddress, method);

            if (parameter.Executor == "First" || parameter.Executor == "FirstOrDefault")
                targetType = typeof(IQueryable<>).MakeGenericType(targetType);

            var result = Client.GetAsync(method).FetchValue(targetType, "GET", absoluteUrl);
            if (parameter.Executor == "First" || parameter.Executor == "FirstOrDefault")
                return (result as IEnumerable).OfType<object>().FirstOrDefault();
            return result;
        }
    }
}