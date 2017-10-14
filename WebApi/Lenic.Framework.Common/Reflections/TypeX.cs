using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Lenic.Framework.Common.Reflections
{
    /// <summary>
    /// 快速类型信息
    /// </summary>
    [DebuggerDisplay("[{Meta.FullName}]")]
    public class TypeX : MetaXBase<Type>
    {
        #region Cache Data

        private static readonly IDictionary<Type, TypeX> _cache = new Dictionary<Type, TypeX>();
        private static readonly ReaderWriterLockSlim _lockCache = new ReaderWriterLockSlim();

        /// <summary>
        /// 获得一个 <see cref="TypeX"/> 类的实例对象。
        /// </summary>
        /// <param name="type">要获得的元数据类型。</param>
        /// <returns>一个 <see cref="TypeX"/> 类的实例对象。</returns>
        public static TypeX GetTypeX(Type type)
        {
            TypeX data = null;
            try
            {
                _lockCache.EnterUpgradeableReadLock();
                if (!_cache.TryGetValue(type, out data))
                {
                    try
                    {
                        _lockCache.EnterWriteLock();

                        data = new TypeX(type);

                        _cache.Add(type, data);
                    }
                    finally
                    {
                        _lockCache.ExitWriteLock(); ;
                    }
                }
            }
            finally
            {
                _lockCache.ExitUpgradeableReadLock();
            }
            return data;
        }

        #region Implicit Convert

        /// <summary>
        /// Performs an implicit conversion from
        /// <see cref="Lenic.Framework.Common.Reflections.TypeX"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Type(TypeX obj)
        {
            return obj.Meta;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Type"/> to
        /// <see cref="Lenic.Framework.Common.Reflections.TypeX"/>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TypeX(Type obj)
        {
            return GetTypeX(obj);
        }

        #endregion Implicit Convert

        #endregion Cache Data

        #region Constructor

        private TypeX(Type type)
        {
            Meta = type;

            InitFields();

            InitProperties();
        }

        private static BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        #endregion Constructor

        #region Field Data

        private IDictionary<string, FieldInfoX[]> fieldMetaDic = null;

        /// <summary>
        /// 获得所有字段列表【子类 => 祖先顺序】
        /// </summary>
        public IEnumerable<FieldInfoX> FullFields
        {
            get
            {
                foreach (var item in fieldMetaDic.Values)
                {
                    foreach (var inner in item)
                    {
                        yield return inner;
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有字段列表。如果继承链上包含相同名称的属性，则只取最子类的字段。
        /// </summary>
        public IEnumerable<FieldInfoX> Fields
        {
            get
            {
                foreach (var item in fieldMetaDic.Values)
                {
                    yield return item[0];
                }
            }
        }

        /// <summary>
        /// 按照从【子类 => 祖先】的顺序查找符合条件的第一个字段。
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <returns>符合条件的字段元数据</returns>
        public FieldInfoX GetField(string name)
        {
            FieldInfoX[] fields = null;
            if (fieldMetaDic.TryGetValue(name, out fields))
            {
                return fields[0];
            }
            return null;
        }

        /// <summary>
        /// 按照从【子类 => 祖先】的顺序查找符合条件的所有字段。
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <returns>符合条件的字段元数据</returns>
        public FieldInfoX[] GetFields(string name)
        {
            FieldInfoX[] fields = null;
            if (fieldMetaDic.TryGetValue(name, out fields))
            {
                return fields;
            }
            return null;
        }

        /// <summary>
        /// 根据 source 对象更新 target 对象，使得两个对象的所有字段属性相同。
        /// </summary>
        /// <param name="target">源对象。</param>
        /// <param name="source">目标对象。</param>
        public void Update<T>(T target, T source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            foreach (FieldInfoX item in FullFields)
            {
                var field = item.GetValue(source);
                item.SetValue(target, field);
            }
        }

        private void InitFields()
        {
            Type t = Meta;
            List<FieldInfoX> fieldMetas = new List<FieldInfoX>();
            do
            {
                fieldMetas.AddRange(
                    from p in t.GetFields(bindingAttr)
                    select new FieldInfoX(p)
                );
                t = t.BaseType;
            } while (t != null && t != typeof(object));

            fieldMetaDic = fieldMetas.GroupBy(p => p.Name).ToDictionary(p => p.Key, p => p.ToArray());
        }

        #endregion Field Data

        #region Property Data

        private IDictionary<string, PropertyInfoX[]> propertyMetaDic = null;

        /// <summary>
        /// 获得所有属性列表【子类 => 祖先】顺序
        /// </summary>
        public IEnumerable<PropertyInfoX> FullProperties
        {
            get
            {
                foreach (var item in propertyMetaDic.Values)
                {
                    foreach (var inner in item)
                    {
                        yield return inner;
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有属性列表。如果继承链上包含相同名称的属性，则只取最子类的属性。
        /// </summary>
        public IEnumerable<PropertyInfoX> Properties
        {
            get
            {
                foreach (var item in propertyMetaDic.Values)
                {
                    yield return item[0];
                }
            }
        }

        /// <summary>
        /// 按照从【子类 => 祖先】的顺序查找符合条件的第一个属性。
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns>符合条件的属性元数据</returns>
        public PropertyInfoX GetProperty(string name)
        {
            PropertyInfoX[] props = null;
            if (propertyMetaDic.TryGetValue(name, out props))
            {
                return props[0];
            }
            return null;
        }

        /// <summary>
        /// 按照从【子类 => 祖先】的顺序查找符合条件的所有属性。
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns>符合条件的属性元数据</returns>
        public PropertyInfoX[] GetProperties(string name)
        {
            PropertyInfoX[] props = null;
            if (propertyMetaDic.TryGetValue(name, out props))
            {
                return props;
            }
            return null;
        }

        private void InitProperties()
        {
            Type t = Meta;
            var propertyMetas = new List<PropertyInfoX>();
            do
            {
                propertyMetas.AddRange(
                    from p in t.GetProperties(bindingAttr)
                    select new PropertyInfoX(p)
                );
                t = t.BaseType;
            } while (t != null && t != typeof(object));

            propertyMetaDic = propertyMetas.GroupBy(p => p.Name).ToDictionary(p => p.Key, p => p.ToArray());
        }

        #endregion Property Data
    }
}