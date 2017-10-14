using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core
{
    /// <summary>
    /// 缺省对象解析器
    /// </summary>
    public class DefaultObjectResolver : IObjectResolver
    {
        #region Private Fields

        private ConcurrentDictionary<Tuple<Type, string>, object> _cache = new ConcurrentDictionary<Tuple<Type, string>, object>();

        #endregion Private Fields

        #region Entrance

        public DefaultObjectResolver()
        {
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 注册泛型定义映射关系项。
        /// </summary>
        /// <param name="targetType">要注册的目标类型。</param>
        /// <param name="implementType">目标类型的实现类型。</param>
        /// <param name="name">映射关系项的名称：缺省为 <c>null</c>。</param>
        public void RegisterGenericTypeDefinition(Type targetType, Type implementType, string name = null)
        {
            _cache.TryAdd(Tuple.Create(targetType, name), implementType);
        }

        /// <summary>
        /// 注册新的映射关系项。
        /// </summary>
        /// <typeparam name="TInterface">要注册的目标类型。</typeparam>
        /// <param name="action">目标类型对象获取委托。</param>
        /// <param name="name">映射关系项的名称：缺省为 <c>null</c>。</param>
        public void Register<TInterface>(Func<IObjectResolver, object> action, string name = null)
        {
            _cache.TryAdd(Tuple.Create(typeof(TInterface), name), action);
        }

        #endregion Business Methods

        #region IObjectResolver 成员

        /// <summary>
        /// 获取指定类型和名称的实例对象。
        /// </summary>
        /// <param name="targetType">要获取的目标类型。</param>
        /// <param name="name">对象注册的名称：缺省为 <c>null</c></param>
        /// <returns>符合条件的实例对象。</returns>
        public object GetInstance(Type targetType, string name = null)
        {
            var key = Tuple.Create(targetType, name);
            object func = null;

            if (_cache.TryGetValue(key, out func))
                return (func as Func<IObjectResolver, object>)(this);

            if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition && _cache.TryGetValue(Tuple.Create(targetType.GetGenericTypeDefinition(), name), out func))
            {
                var createFunc = MakeDelegate(func as Type, targetType.GetGenericArguments());

                Func<IObjectResolver, object> targetGetter = p => createFunc();
                _cache.TryAdd(key, targetGetter);

                return createFunc();
            }

            return null;
        }

        /// <summary>
        /// 获取 {T} 类型和指定名称的实例对象。
        /// </summary>
        /// <typeparam name="T">要获取的目标类型。</typeparam>
        /// <param name="name">对象注册的名称：缺省为 <c>null</c></param>
        /// <returns>符合条件的实例对象。</returns>
        public T GetInstance<T>(string name = null)
        {
            var value = GetInstance(typeof(T), name);
            if (ReferenceEquals(value, null))
                return default(T);
            return (T)value;
        }

        #endregion IObjectResolver 成员

        #region Private Methods

        private Func<object> MakeDelegate(Type implementType, Type[] elementTypes)
        {
            var newExpr = Expression.New(implementType.MakeGenericType(elementTypes));
            var convertExpr = Expression.Convert(newExpr, typeof(object));

            return Expression.Lambda<Func<object>>(convertExpr).Compile();
        }

        #endregion Private Methods
    }
}