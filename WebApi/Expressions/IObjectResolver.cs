using System;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 服务对象解析器接口
    /// </summary>
    public interface IObjectResolver
    {
        /// <summary>
        /// 获取指定类型和名称的实例对象。
        /// </summary>
        /// <param name="targetType">要获取的目标类型。</param>
        /// <param name="name">对象注册的名称：缺省为 <c>null</c></param>
        /// <returns>符合条件的实例对象。</returns>
        object GetInstance(Type targetType, string name = null);

        /// <summary>
        /// 获取 {T} 类型和指定名称的实例对象。
        /// </summary>
        /// <typeparam name="T">要获取的目标类型。</typeparam>
        /// <param name="name">对象注册的名称：缺省为 <c>null</c></param>
        /// <returns>符合条件的实例对象。</returns>
        T GetInstance<T>(string name = null);
    }
}