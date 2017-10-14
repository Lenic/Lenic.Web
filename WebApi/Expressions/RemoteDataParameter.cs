using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 远程对象获取参数类
    /// </summary>
    public class RemoteDataParameter
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置筛选器参数。
        /// </summary>
        public string FilterParameter { get; set; }

        /// <summary>
        /// 获取排序依据参数。
        /// </summary>
        public IList<string> OrderByParameter { get; protected set; }

        /// <summary>
        /// 获取或设置转换器参数。
        /// </summary>
        public string SelectParameter { get; set; }

        /// <summary>
        /// 获取或设置查询过程中跳过的数量。
        /// </summary>
        public string SkipParameter { get; set; }

        /// <summary>
        /// 获取或设置查询过程中获取的数量。
        /// </summary>
        public string TakeParameter { get; set; }

        /// <summary>
        /// 获取或设置扩展信息路径。
        /// </summary>
        public string ExpandParameter { get; set; }

        /// <summary>
        /// 获取或设置最终方法的名称。
        /// </summary>
        public string Executor { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="RemoteDataParameter" /> 类的实例对象。
        /// </summary>
        public RemoteDataParameter()
        {
            OrderByParameter = new List<string>();
        }

        #endregion Entrance

        #region Private Methods

        /// <summary>
        /// 构建查询 Uri.Query 部分。
        /// </summary>
        /// <returns>Uri.Query 部分的字符串。</returns>
        public string BuildUri()
        {
            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(FilterParameter))
            {
                parameters.Add(BuildParameter("$filter", HttpUtility.UrlEncode(FilterParameter)));
            }

            if (!string.IsNullOrWhiteSpace(SelectParameter))
            {
                parameters.Add(BuildParameter("$select", SelectParameter));
            }

            if (!string.IsNullOrWhiteSpace(SkipParameter))
            {
                parameters.Add(BuildParameter("$skip", SkipParameter));
            }

            if (!string.IsNullOrWhiteSpace(TakeParameter))
            {
                parameters.Add(BuildParameter("$top", TakeParameter));
            }

            if (OrderByParameter.Any())
            {
                parameters.Add(BuildParameter("$orderby", string.Join(",", OrderByParameter)));
            }

            if (!string.IsNullOrWhiteSpace(ExpandParameter))
            {
                parameters.Add(BuildParameter("$expand", ExpandParameter));
            }

            return string.Join("&", parameters);
        }

        private static string BuildParameter(string name, string value)
        {
            return name + "=" + value;
        }

        #endregion Private Methods
    }
}