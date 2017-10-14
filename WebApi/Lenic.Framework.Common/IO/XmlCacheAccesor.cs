using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Lenic.Framework.Common.IO
{
    /// <summary>
    /// XML 缓存访问类
    /// </summary>
    [DebuggerStepThrough]
    public sealed class XmlCacheAccesor : IDisposable
    {
        #region Private Fields

        private static IDictionary<string, string> _cache = new Dictionary<string, string>();
        private static string _path = string.Empty;
        private static FileWatcher _watcher = null;

        private bool _contentChanged = false;

        #endregion Private Fields

        #region Private Properties

        private static IDictionary<string, string> Cache
        {
            get
            {
                if (_cache == null && File.Exists(_path))
                {
                    lock (_path)
                    {
                        if (_cache == null && File.Exists(_path))
                            LoadData();
                    }
                }
                return _cache;
            }
        }

        #endregion Private Properties

        #region Entrance

        /// <summary>
        /// 初始化 <see cref="XmlCacheAccesor" /> 类.
        /// </summary>
        static XmlCacheAccesor()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            _path = Path.Combine(_path, "DefaultXmlCache.xml");
            if (File.Exists(_path))
                LoadData();
            else
                File.AppendAllText(_path, string.Empty);
            _watcher = new FileWatcher(_path);
            _watcher.OnXmlFileChanged += () => FileWatcher_OnXmlFileChanged();
        }

        /// <summary>
        /// 初始化新建一个 <see cref="XmlCacheAccesor" /> 类的实例对象.
        /// </summary>
        public XmlCacheAccesor()
        {
            _contentChanged = false;
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 根据指定的键 <see cref="System.String" /> 获取相应的值.
        /// </summary>
        /// <param name="key">待获取值的键.</param>
        /// <returns>相应的值</returns>
        public string this[string key]
        {
            get
            {
                var result = string.Empty;
                if (!Cache.TryGetValue(key, out result))
                {
                    Cache[key] = result;
                }
                return result;
            }
            set
            {
                Cache[key] = value;
                _contentChanged = true;
            }
        }

        /// <summary>
        /// 保存这个对象.
        /// </summary>
        public void Dispose()
        {
            _watcher.CanRaiseChangedEvent = false;

            if (_contentChanged)
            {
                XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

                var root = new XElement("XmlContainer");
                foreach (var item in _cache)
                {
                    root.Add(new XElement(item.Key, item.Value));
                }

                doc.Add(root);
                doc.Save(_path);
            }

            _watcher.CanRaiseChangedEvent = true;
        }

        /// <summary>
        /// 删除指定的键.
        /// </summary>
        /// <param name="key">待删除的键.</param>
        /// <returns>删除结果：<c>true</c> 表示删除成功.</returns>
        public bool Remove(string key)
        {
            bool result = Cache.Remove(key);
            if (result)
                _contentChanged = true;
            return result;
        }

        #endregion Business Methods

        #region Private Methods

        private static void FileWatcher_OnXmlFileChanged()
        {
            _cache.Clear();
            lock (_cache)
            {
                LoadData();
            }
        }

        private static void LoadData()
        {
            var content = File.ReadAllText(_path, Encoding.Default);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var data = XDocument.Parse(content).Root.Elements();
                foreach (var item in data)
                {
                    _cache[item.Name.LocalName] = item.Value;
                }
            }
        }

        #endregion Private Methods
    }
}