using System;
using System.Diagnostics;
using System.Reflection;
using Lenic.Framework.Common.Core;

namespace Lenic.Framework.Common.Reflections
{
    /// <summary>
    /// 属性元数据信息类
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[Name = {Name}, DeclaringType = {Meta.DeclaringType}]")]
    public class PropertyInfoX : MetaXBase<PropertyInfo>, IValue
    {
        /// <summary>
        /// 初始化新建一个 <see cref="PropertyInfoX"/> 类的实例对象。
        /// </summary>
        /// <param name="property">属性元数据</param>
        public PropertyInfoX(PropertyInfo property)
        {
            Meta = property;

            GetValue = MemberDelegateBuilder.NewPropertyGetter(property);
            SetValue = MemberDelegateBuilder.NewPropertySetter(property);
        }

        #region IValue 成员

        /// <summary>
        /// 获取成员快速获值委托。
        /// </summary>
        public Func<object, object> GetValue { get; private set; }

        /// <summary>
        /// 获取成员快速设值委托。
        /// </summary>
        public Action<object, object> SetValue { get; private set; }

        #endregion IValue 成员
    }
}