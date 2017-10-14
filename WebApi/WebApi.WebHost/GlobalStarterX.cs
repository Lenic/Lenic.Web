using Lenic.Framework.Common.Exceptions;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.Logging;
using Lenic.Framework.Common.Messaging;
using Lenic.Web.WebApi.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Lenic.Web.WebApi
{
    internal class GlobalStarterX
    {
        private static readonly Lazy<GlobalStarterX> _lazyInstance = new Lazy<GlobalStarterX>(() => new GlobalStarterX());

        public static GlobalStarterX Instance
        {
            get { return _lazyInstance.Value; }
        }

        private GlobalStarter[] _entryPoints = null;

        private const string _entryPointTooManyError = "程序集【{0}】内的全局启动点太多，请从以下类【{1}】中筛选，只留下一个！";
        private const string _createInstanceError = "创建程序集【{0}】全局启动点类【{1}】的实例对象出现错误，请去掉保证默认构造函数可用！";
        private const string _initError = "初始化程序集【{0}】的入口点发生错误";
        private const string _noEntryPointInfo = "程序集【{0}】没有包含入口点！";

        private GlobalStarterX()
        {
            _entryPoints = ServiceLocator.Current.GetInstance<HttpConfiguration>()
                .Services.GetAssembliesResolver().GetAssemblies().Select(p =>
                {
                    Type[] types = null;
                    try
                    {
                        types = p.GetTypes().Where(r => r.BaseType == typeof(GlobalStarter)).ToArray();
                    }
                    catch (Exception ex)
                    {
                        throw new ModuleConfigException(string.Format("获取程序集【{0}】入口点类型出现异常！", p.Location), ex);
                    }

                    if (types.Length > 1)
                        throw new ModuleConfigException(string.Format(_entryPointTooManyError, p.FullName, string.Join(",", types.Select(r => r.Name))));

                    if (types.Length == 1)
                    {
                        try
                        {
                            return (GlobalStarter)Activator.CreateInstance(types[0]);
                        }
                        catch (Exception e)
                        {
                            throw new ModuleConfigException(string.Format(_createInstanceError, p.FullName, types[0].FullName), e);
                        }
                    }
                    else
                    {
                        ServiceLocator.Current.GetInstance<IMessageRouter>()
                            .NewLog(ErrorCodes.Argument.GetHashCode())
                            .Debug(string.Format(_noEntryPointInfo, p.FullName));

                        return null;
                    }
                })
                .Where(p => p != null)
                .ToArray();
        }

        public void RaiseEvent(Action<GlobalStarter> action)
        {
            _entryPoints
            .ForEach(p =>
            {
                try
                {
                    action(p);
                }
                catch (Exception e)
                {
                    throw new ModuleConfigException(string.Format(_initError, p.GetType().Assembly.FullName), e);
                }
            });
        }
    }
}