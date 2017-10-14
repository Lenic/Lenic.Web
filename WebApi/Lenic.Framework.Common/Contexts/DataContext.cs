using System;
using System.Collections.Generic;

namespace Lenic.Framework.Common.Contexts
{
    /// <summary>
    /// 共享数据上下文
    /// </summary>
    [Serializable]
    public class DataContext
    {
        #region Private Fields

        [ThreadStatic]
        private static DataContext _current;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取当前共享数据上下文的实例对象
        /// </summary>
        public static DataContext Current
        {
            get { return _current; }
            internal set { _current = value; }
        }

        /// <summary>
        /// 获取共享数据上下文中包含的数据集合
        /// </summary>
        public IDictionary<string, object> Items { get; internal set; }

        #endregion Business Properties

        #region Entrance

        internal DataContext()
        {
            this.Items = new Dictionary<string, object>();
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 获取当前共享数据上下文的一个深层拷贝
        /// </summary>
        /// <returns></returns>
        public MirrorContext DepedentClone()
        {
            return new MirrorContext(this);
        }

        /// <summary>
        /// 根据指定的键获取保存在上下文中的数据
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="key">用于获取目标数据的键</param>
        /// <param name="defaultValue">获取目标数据失败后的返回值</param>
        /// <returns>指定的键获取保存在上下文中的数据</returns>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            object value;
            if (this.Items.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 添加或更新指定键在共享数据上下文中的值
        /// </summary>
        /// <param name="key">用于设置目标数据的键</param>
        /// <param name="value">目标数据的具体值</param>
        public void SetValue(string key, object value)
        {
            this.Items[key] = value;
        }

        #endregion Business Methods
    }
}