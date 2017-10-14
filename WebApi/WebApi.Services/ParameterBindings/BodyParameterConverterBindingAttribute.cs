using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Lenic.Web.WebApi.Services.ParameterBindings
{
    /// <summary>
    /// 自定义格式数值绑定特性
    /// </summary>
    public class BodyParameterConverterBindingAttribute : ParameterBindingAttribute
    {
        #region Business Properties

        /// <summary>
        /// 获取转换类类型。
        /// </summary>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// 获取转换类的转换方法名称。
        /// </summary>
        public string MethodName { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="BodyParameterConverterBindingAttribute" /> 类的实例对象。
        /// </summary>
        /// <param name="converterType">转换类类型。</param>
        /// <param name="methodName">转换类的转换方法名称。</param>
        public BodyParameterConverterBindingAttribute(Type converterType, string methodName)
        {
            ConverterType = converterType;
            MethodName = methodName;
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

            return new BodyParameterConverterBinding(parameter)
            {
                CacheKey = Tuple.Create(ConverterType, MethodName),
                ConverterType = ConverterType,
                MethodName = MethodName,
            };
        }

        #endregion Override Members
    }
}