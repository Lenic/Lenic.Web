using System;
using System.Collections;
using System.Linq;
using System.Text;
using Lenic.Framework.Common.Extensions;

namespace Lenic.Framework.Common.Exceptions
{
    /// <summary>
    /// 分级异常类
    /// </summary>
    [Serializable]
    public class LevelException : Exception
    {
        #region Business Properties

        /// <summary>
        /// 获取或设置异常级别信息。
        /// </summary>
        /// <value>
        /// 异常级别信息。
        /// </value>
        public ExceptionLevel Level { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化 <see cref="LevelException" /> 类的新实例。
        /// </summary>
        public LevelException()
            : base()
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="LevelException" /> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public LevelException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="LevelException" /> 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public LevelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="LevelException" /> 类的新实例。
        /// </summary>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public LevelException(Exception innerException)
            : base(string.Format("包装内层错误信息：{0}", (innerException == null ? null : innerException.Message)), innerException)
        {
        }

        #endregion Entrance

        #region Override Methods

        /// <summary>
        /// 获取格式化的错误描述信息。
        /// </summary>
        /// <returns>
        /// 格式化的错误描述信息。
        /// </returns>
        public virtual string GetFormattingMessage()
        {
            StringBuilder sbError = new StringBuilder();

            sbError.AppendLine("分级异常信息：")
                   .Append("异常级别：")
                   .AppendLine(Level.ToString())
                   .Append("文本描述：")
                   .AppendLine(Message);

            if (TargetSite != null)
                sbError.AppendFormat("引发异常的方法：{0}.{1}", TargetSite.DeclaringType.FullName, TargetSite.Name)
                       .AppendLine();

            var data = this.GetExceptions();

            sbError.Append("最内层文本描述：")
                   .AppendLine(data.LastOrDefault().Message);

            var e = data.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Source));
            sbError.Append("错误数据源：")
                   .AppendLine(e == null ? null : e.Source);

            if (data.Any())
            {
                sbError.AppendLine("自定义数据信息：");
                foreach (DictionaryEntry item in Data)
                {
                    sbError.AppendFormat("\t键：【{0}】\t值：【{1}】", item.Key, item.Value)
                           .AppendLine();
                }
            }

            e = data.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.StackTrace));
            sbError.AppendLine("错误跟踪：")
                   .AppendLine(e == null ? null : e.StackTrace);

            return sbError.ToString();
        }

        #endregion Override Methods
    }
}