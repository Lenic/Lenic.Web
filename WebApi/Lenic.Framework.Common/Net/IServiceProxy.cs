using System.Reflection;

namespace Intime.Framework.Common.Net
{
    /// <summary>
    /// 远端服务代理接口
    /// </summary>
    public interface IServiceProxy : IObjectExtendible
    {
        /// <summary>
        /// 执行真正的逻辑代码。
        /// </summary>
        /// <param name="method">待执行的方法元数据。</param>
        /// <param name="args">待执行的方法参数数组。</param>
        /// <returns>
        /// 执行逻辑的返回值。
        /// </returns>
        object Execute(MethodBase method, object[] args);

        /// <summary>
        /// 获取代理的实例对象。
        /// </summary>
        /// <returns>代理的实例对象。</returns>
        object GetProxyInstance();
    }
}