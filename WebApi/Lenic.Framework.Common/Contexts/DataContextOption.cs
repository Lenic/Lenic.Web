namespace Lenic.Framework.Common.Contexts
{
    /// <summary>
    /// 共享数据上下文配置信息
    /// </summary>
    public enum DataContextOption
    {
        /// <summary>
        /// 相同线程上，外层的 DataContext 直接被内层使用
        /// </summary>
        Required,

        /// <summary>
        /// 相同线程上，内层每次都会创建一个全新的 DataContext
        /// </summary>
        RequiresNew,

        /// <summary>
        /// 相同线程上，外层的 DataContextOption 在内层中使被屏蔽掉，内层的当前 DataContextOption 不存在
        /// </summary>
        Suppress
    }
}