using Lenic.Framework.Common.Contexts;
using Lenic.Framework.Common.Extensions;

namespace Lenic.Web.WebApi.Client
{
    /// <summary>
    /// 获取 Http 请求授权信息的扩展方法集合
    /// </summary>
    public static class AuthorizationExtensions
    {
        #region Static Fields

        private static readonly string UserIdName = "UserId";
        private static readonly string LoginAccountName = "LoginAccount";
        private static readonly string UserNameName = "UserName";
        private static readonly string DeptIdName = "DeptId";
        private static readonly string IPName = "IP";
        private static readonly string MachineNameName = "MachineName";
        private static readonly string WarehouseIdName = "WarehouseId";
        private static readonly string WarehouseCodeName = "WarehouseCode";
        private static readonly string WarehouseNameName = "WarehouseName";
        private static readonly string LargeVersionName = "LargeVersion";

        #endregion Static Fields

        #region Business Properties

        /// <summary>
        /// 获取或设置数据上下文中未找到响应信息时，返回的缺省值：缺省值为 <c>null</c> 。
        /// </summary>
        public static string DefaultStringContextValue { get; set; }

        #endregion Business Properties

        #region Get Request Information

        /// <summary>
        /// 获取 Http 请求的用户编号，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求用户编号。</returns>
        public static string GetRequestUserId(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(UserIdName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的用户登录账号，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求用户登录账号。</returns>
        public static string GetRequestLoginAccount(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(LoginAccountName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的用户名，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求用户名。</returns>
        public static string GetRequestUserName(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(UserNameName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的用户所属的部门编号，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为
        /// <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求用户所属的部门编号。</returns>
        public static string GetRequestDeptId(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(DeptIdName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的来源 IP 地址，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为
        /// <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的来源 IP 地址。</returns>
        public static string GetRequestIP(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(IPName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的来源机器名称，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的来源机器名称。</returns>
        public static string GetRequestMachineName(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(MachineNameName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的来源仓库编号，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的来源仓库编号。</returns>
        public static string GetRequestWarehouseId(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(WarehouseIdName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的来源仓库识别码，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的来源仓库识别码。</returns>
        public static string GetRequestWarehouseCode(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(WarehouseCodeName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的来源仓库名称，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的来源仓库名称。</returns>
        public static string GetRequestWarehouseName(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(WarehouseNameName, DefaultStringContextValue) as string;
        }

        /// <summary>
        /// 获取 Http 请求的客户端大版本号，未找到时返回 <see cref="DefaultStringContextValue"/> 属性的值（缺省为 <c>null</c>）。
        /// </summary>
        /// <param name="context">当前数据上下文的实例对象。</param>
        /// <returns>获取得到的 Http 请求的客户端大版本号。</returns>
        public static string GetRequestLargeVersion(this DataContext context)
        {
            return DataContext.Current.Items.GetValueX(LargeVersionName, DefaultStringContextValue) as string;
        }

        #endregion Get Request Information
    }
}