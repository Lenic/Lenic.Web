using System;
using System.Collections.Concurrent;
using System.Linq;
using Lenic.Framework.Caching;
using Lenic.Framework.Common.Security;
using Lenic.Web.WebApi.Authorization.Properties;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Lenic.Web.WebApi.Authorization
{
    /// <summary>
    /// 缺省身份令牌验证器
    /// </summary>
    public sealed class DefaultAuthenticationTokenValidator : IAuthenticationTokenValidator
    {
        #region Private Static Fields

        private static readonly RsaHelper Encoder = new RsaHelper(Resources.RasKey);

        #endregion Private Static Fields

        #region Private Fields

        private ICacheContainer _cache = null;

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="DefaultAuthenticationTokenValidator" /> 类的实例对象。
        /// </summary>
        public DefaultAuthenticationTokenValidator()
        {
            _cache = ServiceLocator.Current.GetInstance<ICacheContainer>();
        }

        #endregion Entrance

        #region IAuthenticationTokenValidator 成员

        /// <summary>
        /// 验证身份令牌是否通过：<c>true</c> 表示验证通过。
        /// </summary>
        /// <param name="token">待验证的身份令牌。</param>
        /// <returns><c>true</c> 表示验证通过，否则返回 <c>false</c> 。</returns>
        public bool Validate(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            RequestInfo info = GetRequestInfo(token);
            if (info == null)
                return false;

            var item = _cache.GetValueX<ConcurrentDictionary<string, DateTime>>(GetCacheKey(info), throwIfException: false);
            if (item == null)
                return false;

            DateTime original = DateTime.MinValue;
            if (!item.TryGetValue(info.Code, out original))
                return false;

            if (original.Add(SlideDue) >= DateTime.Now)
            {
                item.TryUpdate(info.Code, DateTime.Now, original);
                return true;
            }

            item.TryRemove(info.Code, out original);
            return false;
        }

        #endregion IAuthenticationTokenValidator 成员

        #region IAuthenticationTokenRegister 成员

        /// <summary>
        /// 获取或设置滑动过期的间隔
        /// </summary>
        public TimeSpan SlideDue { get; set; }

        /// <summary>
        /// 向指定用户注册一个新的身份令牌。
        /// </summary>
        /// <param name="identity">用户的唯一标识。</param>
        /// <param name="token">产生的身份令牌。</param>
        /// <returns><c>true</c> 表示注册成功；否则返回 <c>false</c> 。</returns>
        public bool Regist(string identity, out string token)
        {
            RequestInfo info = GetRequestInfo(identity);
            if (info != null)
            {
                ILoginValidator validator = null;
                try
                {
                    validator = ServiceLocator.Current.GetInstance<ILoginValidator>(info.Client);
                }
                catch { }

                if (validator != null && validator.Validate(info.Name, info.Code))
                {
                    var data = _cache.GetOrAddXWithFunc<ConcurrentDictionary<string, DateTime>>(GetCacheKey(info), () =>
                        Tuple.Create(new ConcurrentDictionary<string, DateTime>(Environment.ProcessorCount * 2, 16), Expiration.FromSliding(SlideDue)));

                    return data.TryAdd(token = Guid.NewGuid().ToString("N"), DateTime.Now);
                }
            }

            token = string.Empty;
            return false;
        }

        #endregion IAuthenticationTokenRegister 成员

        #region Private Methods

        private string GetCacheKey(RequestInfo info)
        {
            return string.Format("{0}_{1}", info.Client, info.Name);
        }

        private RequestInfo GetRequestInfo(string identity)
        {
            RequestInfo info = null;

            try
            {
                info = JsonConvert.DeserializeObject<RequestInfo>(Encoder.Decrypt(identity));
            }
            catch { }

            return info;
        }

        #endregion Private Methods

        #region Private Criteria Class

        /// <summary>
        /// 请求信息类
        /// </summary>
        private class RequestInfo
        {
            /// <summary>
            /// 获取或设置客户端信息。
            /// </summary>
            public string Client { get; set; }

            /// <summary>
            /// 获取或设置识别码信息。
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// 获取或设置名称信息。
            /// </summary>
            public string Name { get; set; }
        }

        #endregion Private Criteria Class
    }
}