namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// 定义用于创建和执行 <see cref="System.Linq.IQueryable" /> 对象所描述的查询的方法。
    /// </summary>
    /// <typeparam name="T">提供可以查询的元素类型。</typeparam>
    public interface IQueryProvider<T> : System.Linq.IQueryProvider
    {
        /// <summary>
        /// 获取或设置远程对象获取器实例对象。
        /// </summary>
        IRemoteDataFetcher DataFetcher { get; set; }
    }
}