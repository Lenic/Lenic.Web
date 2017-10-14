using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 表达式树向 OData 协议转换的接口
    /// </summary>
    public interface IExpressionWriter
    {
        /// <summary>
        /// 转换表达式树到 OData 协议字符串。
        /// </summary>
        /// <param name="expression">一个需要转化的 <see cref="Expression" /> 的实例对象。</param>
        /// <returns>转换完成的目标字符串：OData 格式</returns>
        string Write(Expression expression);
    }
}