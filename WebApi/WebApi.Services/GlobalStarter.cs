namespace Lenic.Web.WebApi.Services
{
    /// <summary>
    /// 全局启动器【每个有控制器（Controller）的程序集只触发一次】
    /// </summary>
    public abstract class GlobalStarter
    {
        /// <summary>
        /// 引发当前程序集的初始化操作：该操作在初始化 IOC 基本配置，加载默认的配置管道后触发，触发索引【1】。
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 引发当前程序集的配置完成操作：该操作在所有配置注册后触发，触发索引【2】。
        /// </summary>
        public virtual void Complete()
        {
        }
    }
}