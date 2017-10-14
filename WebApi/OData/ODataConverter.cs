using Lenic.Web.WebApi.Expressions;
using Lenic.Web.WebApi.HttpBinding;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace Lenic.Web.WebApi.OData
{
    /// <summary>
    /// OData 查询转换入口类
    /// </summary>
    public static class ODataConverter
    {
        #region Private Fields

        private static Lazy<ServicesContainer> _container = null;

        #endregion Private Fields

        #region Business Properties

        internal static ServicesContainer Container
        {
            get { return _container.Value; }
        }

        #endregion Business Properties

        #region Entrance

        static ODataConverter()
        {
            _container = new Lazy<ServicesContainer>(() => ServiceLocator.Current.GetInstance<HttpConfiguration>().Services, true);
        }

        /// <summary>
        /// 创建一个 OData 转换类的实例对象。
        /// </summary>
        /// <typeparam name="T">OData 方法所在类类型</typeparam>
        /// <param name="expr">查询的 Lambda 表达式树。</param>
        /// <param name="method">当前运行的方法元数据，一般使用：MethodBase.GetCurrentMethod() 获取。</param>
        /// <returns>一个 OData 转换类的实例对象。</returns>
        public static ODataConverter<T> CreateInstance<T>(LambdaExpression expr, MethodBase method)
        {
            return new ODataConverter<T>(expr, method);
        }

        #endregion Entrance
    }

    /// <summary>
    /// OData 查询转换类
    /// </summary>
    /// <typeparam name="T">转换类支持的查询元素类型</typeparam>
    public class ODataConverter<T>
    {
        #region Static Fields

        private static MethodInfo _createODataQueryOptions = typeof(ODataQueryPackager).GetMethod("CreateODataQueryOptions", BindingFlags.NonPublic | BindingFlags.Static);
        private static ConstructorInfo _ctor = typeof(ODataConventionModelBuilder).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(HttpConfiguration), typeof(bool) }, null);

        #endregion Static Fields

        #region Private Fields

        private HttpRequestMessage _request = null;
        private HttpActionDescriptor _action = null;

        #endregion Private Fields

        #region Entrance

        internal ODataConverter(LambdaExpression expr, MethodBase method)
        {
            IExpressionProcessor processor = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionProcessor>();
            processor.Writer = RemoteObjectContext.DefaultObjectResolver.GetInstance<IExpressionWriter>();
            processor.DataParameter = new RemoteDataParameter();
            processor.Build(expr.Body as MethodCallExpression);

            var uri = new Uri("http://localhost:60000?" + processor.DataParameter.BuildUri());
            _request = new HttpRequestMessage(HttpMethod.Get, uri);

            var controller = ODataConverter.Container.GetHttpControllerSelector().GetControllerMapping().Values.Where(p => p.ControllerType == typeof(T)).Single();
            var mapping = ODataConverter.Container.GetActionSelector().GetActionMapping(controller);
            var action = GetAction(method);

            _action = mapping[action].First();
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 附加当前查询到参数 <paramref name="query"/> 指定的查询上。
        /// </summary>
        /// <param name="query">预支的查询。</param>
        /// <returns>附加查询之后的查询。</returns>
        public IQueryable ApplyTo(IQueryable query)
        {
            return ApplyTo(query, null);
        }

        /// <summary>
        /// 附加当前查询到参数 <paramref name="query"/> 指定的查询上。
        /// </summary>
        /// <param name="query">预支的查询。</param>
        /// <param name="querySettings">查询的一些设置信息。</param>
        /// <returns>附加查询之后的查询。</returns>
        public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
        {
            var func = (Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>)
                Delegate.CreateDelegate(typeof(Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>), _createODataQueryOptions.MakeGenericMethod(new Type[] { query.ElementType }));

            var model = ODataQueryPackager.GetEdmModel(_action, query.ElementType);
            ODataQueryOptions options = func(new ODataQueryContext(model, query.ElementType), _request);
            if (querySettings == null)
                return options.ApplyTo(query);
            else
                return options.ApplyTo(query, querySettings);
        }

        #endregion Business Methods

        #region Private Methods

        private string GetAction(MethodBase method)
        {
            var actionName = method.GetCustomAttribute<Lenic.Web.WebApi.HttpBinding.ActionNameAttribute>();
            if (actionName != null)
                return actionName.Value;

            return method.Name;
        }

        #endregion Private Methods
    }
}