using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.Framework.Common.Net
{
    /// <summary>
    /// 消息发送对象
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    public class Message
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置消息封送的类名称信息。
        /// </summary>
        /// <value>
        /// 类名称信息。
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// 获取或设置消息封送的类序列化后的字符串。
        /// </summary>
        /// <value>
        /// 类序列化后的字符串。
        /// </value>
        public string Data { get; set; }

        /// <summary>
        /// 获取对象扩展属性。
        /// </summary>
        /// <value>
        /// 对象扩展属性集合。
        /// </value>
        public List<NameValue> Tag { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="Message"/> 类的实例对象。
        /// </summary>
        public Message()
        {
            Tag = new List<NameValue>();
        }

        #endregion Entrance

        #region Private Class

        /// <summary>
        /// 键值对信息类
        /// </summary>
        [Serializable]
        public class NameValue
        {
            /// <summary>
            /// 名称信息
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 名称对应的值信息
            /// </summary>
            public string Value { get; set; }
        }

        #endregion Private Class
    }
}