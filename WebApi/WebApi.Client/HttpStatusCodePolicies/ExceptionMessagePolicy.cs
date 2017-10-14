using Lenic.Framework.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;

namespace Lenic.Web.WebApi.Client.HttpStatusCodePolicies
{
    public class ExceptionMessagePolicy : IHttpStatusCodePolicy
    {
        #region IHttpStatusCodePolicy 成员

        public HttpStatusCode Code
        {
            get { return HttpStatusCode.ExpectationFailed; }
        }

        public object Execute(HttpResponseMessage response)
        {
            if (response.StatusCode != Code)
                throw new ArgumentException("[ExceptionMessagePolicy].[Execute].response.StatusCode != HttpStatusCode.ExpectationFailed.");

            ErrorMessage msg = null;
            string responseString = null;
            try
            {
                if (response.Content.Headers.ContentType.MediaType.Contains("json"))
                {
                    var token = JToken.Parse(responseString = response.Content.ReadAsStringAsync().Result);
                    if (token.Type == JTokenType.Object)
                    {
                        var obj = token as JObject;
                        while (true)
                        {
                            var jp = obj.Property("InnerException");
                            if (jp != null)
                            {
                                var value = jp.Value.Value<JObject>();
                                if (value != null)
                                    obj = value;
                                else
                                    break;
                            }
                            else
                                break;
                        }

                        msg = obj.ToObject<ErrorMessage>();
                    }
                }
                else
                    msg = response.Content.ReadAsAsync<ErrorMessage>().Result;
            }
            catch
            {
                msg = JsonConvert.DeserializeObject<ErrorMessage>(responseString ?? response.Content.ReadAsStringAsync().Result);
            }

            throw new BusinessException(msg.ExceptionMessage ?? msg.Message);
        }

        #endregion IHttpStatusCodePolicy 成员

        #region Private Classes

        private class ErrorMessage
        {
            #region Public Properties

            public string Message { get; set; }

            public string ExceptionMessage { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}