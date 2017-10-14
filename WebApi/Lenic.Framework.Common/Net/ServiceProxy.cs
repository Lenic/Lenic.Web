using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Intime.Framework.Common.Net
{
    /// <summary>
    /// 远端服务代理虚基类
    /// </summary>
    [DebuggerStepThrough]
    public abstract class ServiceProxy : RealProxy, IServiceProxy
    {
        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="ServiceProxy{TInterface}" /> 类的实例对象。
        /// </summary>
        /// <param name="type">要代理的接口类型。</param>
        protected ServiceProxy(Type type)
            : base(type)
        {
            Tag = new Dictionary<string, string>();
        }

        #endregion Entrance

        #region Override Methods

        /// <summary>
        /// 当在派生类中重写时，对当前实例所表示的远程对象调用在所提供的 <see cref="T:System.Runtime.Remoting.Messaging.IMessage" /> 中指定的方法。
        /// </summary>
        /// <param name="msg"><see cref="T:System.Runtime.Remoting.Messaging.IMessage" />，包含有关方法调用的信息的 <see cref="T:System.Collections.IDictionary" />。</param>
        /// <returns>
        /// 调用的方法所返回的消息，包含返回值和所有 out 或 ref 参数。
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure" />
        ///   </PermissionSet>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = msg as IMethodCallMessage;
            object[] copiedArgs = new object[methodCall.Args.Length];
            methodCall.Args.CopyTo(copiedArgs, 0);
            object returnValue = Execute(methodCall.MethodBase, copiedArgs);
            IMethodReturnMessage methodReturn = new ReturnMessage(returnValue, copiedArgs, copiedArgs.Length, methodCall.LogicalCallContext, methodCall);

            return methodReturn;
        }

        #endregion Override Methods

        #region IServiceProxy 成员

        /// <summary>
        /// 执行真正的逻辑代码。
        /// </summary>
        /// <param name="method">待执行的方法元数据。</param>
        /// <param name="args">待执行的方法参数数组。</param>
        /// <returns>
        /// 执行逻辑的返回值。
        /// </returns>
        public abstract object Execute(MethodBase method, object[] args);

        /// <summary>
        /// 获取代理的实例对象。
        /// </summary>
        /// <returns>
        /// 代理的实例对象。
        /// </returns>
        public object GetProxyInstance()
        {
            return GetTransparentProxy();
        }

        #endregion IServiceProxy 成员

        #region IObjectExtendible 成员

        /// <summary>
        /// 获取对象扩展属性。
        /// </summary>
        /// <value>
        /// 对象扩展属性字典集合。
        /// </value>
        public IDictionary<string, string> Tag { get; private set; }

        #endregion IObjectExtendible 成员
    }
}