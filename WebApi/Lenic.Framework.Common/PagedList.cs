using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lenic.Framework.Common
{
    /// <summary>
    /// 分页器类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    [Serializable]
    public sealed class PagedList<T>
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置分页的具体数据内容。
        /// </summary>
        [DataMember]
        public List<T> ContentList { get; set; }

        /// <summary>
        /// 获取或设置每页的最大数据容量。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 获取或设置未分页前的数据总数量。
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="PagedList{T}"/> 类的实例对象。
        /// </summary>
        public PagedList()
        {
            ContentList = new List<T>();
        }

        /// <summary>
        /// 初始化新建一个 <see cref="PagedList{T}" /> 类的实例对象。
        /// </summary>
        /// <param name="contents">分页的具体数据内容。</param>
        /// <param name="recordCount">未分页前的数据总数量。</param>
        /// <param name="pageSize">每页的最大数据容量。</param>
        public PagedList(List<T> contents, int recordCount, int pageSize)
            : this()
        {
            ContentList = contents ?? new List<T>();
            TotalCount = recordCount;
            PageSize = pageSize;
        }

        #endregion Entrance
    }
}