namespace Lenic.Framework.Common.Logging
{
    /// <summary>
    /// 对象变更类型
    /// </summary>
    public enum ObjectChangeType
    {
        /// <summary>
        /// 【缺省】添加
        /// </summary>
        Add = 0,

        /// <summary>
        /// 更新
        /// </summary>
        Update = 2,

        /// <summary>
        /// 更新某些属性
        /// </summary>
        UpdateProperties = 4,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 5,
    }
}