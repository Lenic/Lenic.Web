using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Lenic.Framework.Common.Reflections
{
    /// <summary>
    /// 泛型快速成员访问接口
    /// </summary>
    /// <typeparam name="T">从 <see cref="System.Reflection.MemberInfo"/> 类继承的类型</typeparam>
    public interface IMetaX<T> where T : MemberInfo
    {
        /// <summary>
        /// 获取成员元数据。
        /// </summary>
        T Meta { get; }

        /// <summary>
        /// 获取成员元数据名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取指定类型的特性（Attribute）数组。
        /// </summary>
        /// <typeparam name="TAttribute">具体的特性类型</typeparam>
        /// <param name="inherit"><c>true</c> 表示在继承链上查找；否则表示只在当前成员上查找。</param>
        /// <returns>指定类型的特性（Attribute）数组。</returns>
        TAttribute[] GetCustomAttributes<TAttribute>(bool inherit = false);
    }

    /// <summary>
    /// 成员快速获值设值接口
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// 获取成员快速获值委托。
        /// </summary>
        Func<object, object> GetValue { get; }

        /// <summary>
        /// 获取成员快速设值委托。
        /// </summary>
        Action<object, object> SetValue { get; }
    }

    /// <summary>
    /// 泛型快速成员访问基类
    /// </summary>
    /// <typeparam name="T">从 <see cref="System.Reflection.MemberInfo"/> 类继承的类型</typeparam>
    [Serializable]
    [DebuggerStepThrough]
    public class MetaXBase<T> : IMetaX<T> where T : MemberInfo
    {
        #region Private Fields

        private IDictionary<Tuple<T, Type, bool>, object[]> _attributes =
            new Dictionary<Tuple<T, Type, bool>, object[]>();

        private ReaderWriterLockSlim _lockCache = new ReaderWriterLockSlim();

        #endregion Private Fields

        #region IMetaX<T> 成员

        /// <summary>
        /// 获取成员元数据。
        /// </summary>
        public T Meta { get; protected set; }

        /// <summary>
        /// 获取成员元数据名称。
        /// </summary>
        public string Name
        {
            get { return Meta.Name; }
        }

        /// <summary>
        /// 获取指定类型的特性（Attribute）数组。
        /// </summary>
        /// <typeparam name="TAttribute">具体的特性类型</typeparam>
        /// <param name="inherit"><c>true</c> 表示在继承链上查找；否则表示只在当前成员上查找。</param>
        /// <returns>指定类型的特性（Attribute）数组。</returns>
        public TAttribute[] GetCustomAttributes<TAttribute>(bool inherit = false)
        {
            var key = Tuple.Create(Meta, typeof(TAttribute), inherit);
            object[] data = null;

            try
            {
                _lockCache.EnterUpgradeableReadLock();
                if (!_attributes.TryGetValue(key, out data))
                {
                    try
                    {
                        _lockCache.EnterWriteLock();

                        data = Meta.GetCustomAttributes(typeof(TAttribute), inherit);

                        _attributes.Add(key, data);
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
            return data as TAttribute[];
        }

        #endregion IMetaX<T> 成员

        #region Override Object

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// 	<paramref name="obj"/> 参数为 null。
        /// </exception>
        public override bool Equals(object obj)
        {
            var other = obj as MetaXBase<T>;

            if (other == null)
                return false;

            return Meta.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Meta.GetHashCode();
        }

        #endregion Override Object
    }
}