using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// XML 格式数值绑定特性
    /// </summary>
    public class XmlParameterBindingAttribute : ParameterBindingAttribute
    {
        #region Business Properties

        /// <summary>
        /// 获取目标类类型。
        /// </summary>
        public Type TargetType { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="XmlParameterBindingAttribute" /> 类的实例对象。
        /// </summary>
        public XmlParameterBindingAttribute()
        {
        }

        #endregion Entrance

        #region Override Members

        /// <summary>
        /// 获取参数绑定。
        /// </summary>
        /// <param name="parameter">参数说明。</param>
        /// <returns>参数绑定。</returns>
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            parameter.ParameterBinderAttribute = this;

            return new XmlParameterBinding(parameter)
            {
                TargetType = TargetType,
            };
        }

        #endregion Override Members
    }
}