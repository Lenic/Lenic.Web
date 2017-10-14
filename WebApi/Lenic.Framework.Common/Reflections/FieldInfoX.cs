using System;
using System.Diagnostics;
using System.Reflection;
using Lenic.Framework.Common.Core;

namespace Lenic.Framework.Common.Reflections
{
    /// <summary>
    /// 字段元数据信息类
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[Name = {Name}, DeclaringType = {Meta.DeclaringType}]")]
    public class FieldInfoX : MetaXBase<FieldInfo>, IValue
    {
        /// <summary>
        /// 初始化新建一个 <see cref="FieldInfoX"/> 类的实例对象。
        /// </summary>
        /// <param name="field">字段元数据。</param>
        public FieldInfoX(FieldInfo field)
        {
            Meta = field;

            GetValue = MemberDelegateBuilder.NewFieldGetter(field);
            SetValue = MemberDelegateBuilder.NewFieldSetter(field);
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