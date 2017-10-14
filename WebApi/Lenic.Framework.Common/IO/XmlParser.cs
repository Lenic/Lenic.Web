using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Lenic.Framework.Common.IO
{
    /// <summary>
    /// 配置文件分析方式
    /// </summary>
    public enum XmlParserTypes
    {
        /// <summary>
        /// 【缺省值】自适应检索方式（分析顺序：Attriburtes、Children）
        /// </summary>
        Auto = 0,

        /// <summary>
        /// 从 Attributes 中检索
        /// </summary>
        Attribute = 1,

        /// <summary>
        /// 从 Children 中检索
        /// </summary>
        InnerElement = 2,
    }

    /// <summary>
    /// 尝试从字符串中解析 T 类型, 分析成功时返回 <c>true</c> , 分析结果在 result 中.
    /// </summary>
    /// <typeparam name="T">待分析的目标类型.</typeparam>
    /// <param name="obj">待分析的源字符串.</param>
    /// <param name="result">分析完成的结果.</param>
    /// <returns><c>true</c> 表示分析成功; 否则返回 <c>false</c> .</returns>
    public delegate bool TryParse<T>(string obj, out T result);

    /// <summary>
    /// 配置文件分析基类
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("Type = {Type}, InnerCount = {InnerParsers.Length}")]
    public class XmlParser
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XElement element = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, string> attributes = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, string> children = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XmlParser[] items = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string type = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XmlParser[] innerParsers = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string text = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool needReload = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FileWatcher fileWatcher = null;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 获取获设置文件路径.
        /// </summary>
        public string FilePath = string.Empty;

        private XElement Element
        {
            get
            {
                if (needReload)
                {
                    lock (this)
                    {
                        if (needReload)
                        {
                            InitInstance(LoadXmlFile());
                            needReload = false;
                        }
                    }
                }
                if (element == null)
                    throw new NullReferenceException("The current element is not effective.");
                return element;
            }
        }

        /// <summary>
        /// 获取当前节点的类型.
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// 获取当前节点的串连文本内容.
        /// </summary>
        public string Text
        {
            get
            {
                if (text == null)
                {
                    lock (this)
                    {
                        if (text == null)
                            text = Element.Value;
                    }
                }
                return text;
            }
        }

        /// <summary>
        /// 获取当前节点的所有属性集合.
        /// </summary>
        [DebuggerDisplay("Count = {Attributes.Count}")]
        public Dictionary<string, string> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    lock (this)
                    {
                        if (attributes == null)
                        {
                            attributes = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                            foreach (var item in Element.Attributes())
                            {
                                if (!attributes.ContainsKey(item.Name.LocalName))
                                    attributes.Add(item.Name.LocalName, item.Value);
                            }
                        }
                    }
                }
                return attributes;
            }
        }

        /// <summary>
        /// 获取当前节点的所有内含元素集合（不包含 items 节点）.
        /// </summary>
        [DebuggerDisplay("Count = {Children.Count}")]
        public Dictionary<string, string> Children
        {
            get
            {
                if (children == null)
                {
                    lock (this)
                    {
                        if (children == null)
                        {
                            children = new Dictionary<string, string>();

                            var data = Element.Elements().Where(p => !p.HasElements);
                            foreach (var item in data)
                                children.Add(item.Name.LocalName, item.Value);
                        }
                    }
                }
                return children;
            }
        }

        /// <summary>
        /// 获取当前节点的所有内含元素集合（所有节点）.
        /// </summary>
        [DebuggerDisplay("Length = {InnerParsers.Length}")]
        public XmlParser[] InnerParsers
        {
            get
            {
                if (innerParsers == null)
                {
                    lock (this)
                    {
                        if (innerParsers == null)
                        {
                            innerParsers = Element.Elements().Select(p => new XmlParser(p)).ToArray();
                        }
                    }
                }
                return innerParsers;
            }
        }

        #endregion Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="XmlParser"/> 类的实例对象.
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        /// <returns>一个 <see cref="XmlParser"/> 类的实例对象</returns>
        public static XmlParser Load(string filePath)
        {
            return new XmlParser(filePath);
        }

        /// <summary>
        /// 初始化新建一个 <see cref="XmlParser"/> 类的实例对象.
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        public XmlParser(string filePath)
        {
            this.FilePath = Path.GetFullPath(filePath);

            InitInstance(LoadXmlFile());
            fileWatcher = new FileWatcher(FilePath);
            fileWatcher.OnXmlFileChanged += () => OnXmlFileChanged();
        }

        private XElement LoadXmlFile()
        {
            return XDocument.Load(FilePath).Root;
        }

        private XmlParser(XElement element)
        {
            InitInstance(element);
            fileWatcher = new FileWatcher();
            fileWatcher.OnXmlFileChanged += () => OnXmlFileChanged();
        }

        private void InitInstance(XElement reference)
        {
            this.element = reference;
            this.type = reference.Name.LocalName;

            this.OnXmlFileChanged = () =>
            {
                Monitor.Enter(this);

                if (items != null)
                {
                    foreach (var item in items)
                        item.fileWatcher.RaiseChangedEvent();
                    items = null;
                }

                if (innerParsers != null)
                {
                    foreach (var item in innerParsers)
                        item.fileWatcher.RaiseChangedEvent();
                    innerParsers = null;
                }

                element = null;
                type = null;
                text = null;
                attributes = null;
                children = null;
                if (!string.IsNullOrEmpty(FilePath))
                    needReload = true;

                Monitor.Exit(this);
            };
        }

        #endregion Entrance

        #region Events

        /// <summary>
        /// 在原始 XML 文件内容发生改变的时候发生
        /// </summary>
        public event Action OnXmlFileChanged;

        /// <summary>
        /// 引发配置项变化的事件。
        /// </summary>
        public void RaiseChangedEvent()
        {
            fileWatcher.RaiseChangedEvent();
        }

        #endregion Events

        #region Parse Integer

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找，缺省值为 <c>int.MinValue</c> ，并且忽略大小写）.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <returns>检索结果.</returns>
        public int GetInteger(string name)
        {
            return GetInteger(name, int.MinValue, XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找）.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <returns>检索结果.</returns>
        public int GetInteger(string name, int defaultValue)
        {
            return GetInteger(name, defaultValue, XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <param name="type">属性值检索方式.</param>
        /// <returns>检索结果.</returns>
        public int GetInteger(string name, int defaultValue, XmlParserTypes type)
        {
            var value = GetString(name, null, type);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            int result = 0;
            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        #endregion Parse Integer

        #region Parse T

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找，缺省值为 <c>default(T)</c> ，并且忽略大小写）.
        /// </summary>
        /// <typeparam name="T">待分析的类型.</typeparam>
        /// <param name="name">属性名称.</param>
        /// <param name="tryParse">字符串尝试分析委托.</param>
        /// <returns>检索结果.</returns>
        public T GetValue<T>(string name, TryParse<T> tryParse)
        {
            return GetValue<T>(name, tryParse, default(T), XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找）.
        /// </summary>
        /// <typeparam name="T">待分析的类型.</typeparam>
        /// <param name="name">属性名称.</param>
        /// <param name="tryParse">字符串尝试分析委托.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <returns>检索结果.</returns>
        public T GetValue<T>(string name, TryParse<T> tryParse, T defaultValue)
        {
            return GetValue<T>(name, tryParse, defaultValue, XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值.
        /// </summary>
        /// <typeparam name="T">待分析的类型.</typeparam>
        /// <param name="name">属性名称.</param>
        /// <param name="tryParse">字符串尝试分析委托.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <param name="type">属性值检索方式.</param>
        /// <returns>检索结果.</returns>
        public T GetValue<T>(string name, TryParse<T> tryParse, T defaultValue, XmlParserTypes type)
        {
            var value = GetString(name, null, type);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            T result = default(T);
            if (tryParse(value, out result))
                return result;

            return defaultValue;
        }

        #endregion Parse T

        #region Parse String

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找，缺省值为 <c>null</c> ，并且忽略大小写）.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <returns>检索结果.</returns>
        public string GetString(string name)
        {
            return GetString(name, null, XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值（自适应在 Attribute 和 InnerElement 中寻找）.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <returns>检索结果.</returns>
        public string GetString(string name, string defaultValue)
        {
            return GetString(name, defaultValue, XmlParserTypes.Auto);
        }

        /// <summary>
        /// 根据指定的条件获取属性的值.
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <param name="defaultValue">未找到属性时返回的缺省值.</param>
        /// <param name="type">属性值检索方式.</param>
        /// <returns>检索结果.</returns>
        public string GetString(string name, string defaultValue, XmlParserTypes type)
        {
            var target = string.Empty;

            if (type == XmlParserTypes.Attribute)
                Attributes.TryGetValue(name, out target);
            else if (type == XmlParserTypes.InnerElement)
                Children.TryGetValue(name, out target);
            else if (type == XmlParserTypes.Auto)
            {
                if (Attributes.TryGetValue(name, out target))
                    return target;
                else if (Children.TryGetValue(name, out target))
                    return target;
            }
            else
                throw new NotSupportedException("error enum");

            return target;
        }

        #endregion Parse String
    }
}