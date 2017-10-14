using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 远程对象获取器接口构建器
    /// </summary>
    public interface IExpressionProcessor
    {
        /// <summary>
        /// 获取或设置待构建的远程对象检索用参数。
        /// </summary>
        RemoteDataParameter DataParameter { get; set; }

        /// <summary>
        /// 获取或设置 OData 协议转换接口的实例对象。
        /// </summary>
        IExpressionWriter Writer { get; set; }

        /// <summary>
        /// 构建远程对象获取器接口实例对象。
        /// </summary>
        /// <param name="methodCall">一个方法调用的表达式树。</param>
        void Build(MethodCallExpression methodCall);
    }
}