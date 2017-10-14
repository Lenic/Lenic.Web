using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// 枚举类型的扩展
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumExtensions
    {
        private static IDictionary<Tuple<Enum, Type>, object[]> cache = new Dictionary<Tuple<Enum, Type>, object[]>();
        private static ReaderWriterLockSlim lockCache = new ReaderWriterLockSlim();

        /// <summary>
        /// 获得当前枚举实例的 <see cref="System.ComponentModel.DescriptionAttribute"/> 属性值.
        /// </summary>
        /// <param name="value">当前枚举实例.</param>
        /// <returns>当前枚举实例的 <see cref="System.ComponentModel.DescriptionAttribute"/> 属性值.</returns>
        public static string GetDescription(this Enum value)
        {
            var item = GetAttribute<DescriptionAttribute>(value).FirstOrDefault();
            if (item != null)
                return item.Description;
            return string.Empty;
        }

        /// <summary>
        /// 获得枚举字段关联的指定特性(Attribute)的列表。
        /// </summary>
        /// <typeparam name="TAttribute">扩展特性类型</typeparam>
        /// <param name="value">当前枚举实例。</param>
        /// <returns>枚举字段的特性列表</returns>
        public static TAttribute[] GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var key = Tuple.Create(value, typeof(TAttribute));
            object[] data = null;

            try
            {
                lockCache.EnterUpgradeableReadLock();
                if (!cache.TryGetValue(key, out data))
                {
                    try
                    {
                        lockCache.EnterWriteLock();

                        FieldInfo field = value.GetType().GetField(value.ToString());
                        data = field.GetCustomAttributes(typeof(TAttribute), false);

                        cache.Add(key, data);
                    }
                    finally
                    {
                        lockCache.ExitWriteLock(); ;
                    }
                }
            }
            finally
            {
                lockCache.ExitUpgradeableReadLock();
            }
            return data as TAttribute[];
        }
    }
}