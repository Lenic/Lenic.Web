using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Lenic.Framework.Common.Core;
using Microsoft.CSharp;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// 字符串扩展方法集合
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 获取下一个流水号。
        /// </summary>
        /// <param name="data">一个字符串的实例对象。</param>
        /// <param name="increment">下一个流水号要添加的增量。</param>
        /// <returns>获取得到的下一个流水号。</returns>
        public static string GetNextFlowNumber(this string data, int increment = 1)
        {
            var obj = new FlowNumberGenerator(data);
            obj.Build(increment);
            return obj.ToString();
        }

        /// <summary>
        /// * 编译动态程序集
        /// </summary>
        /// <param name="sourceCode">源码</param>
        /// <param name="assemblyNames">所需要的程序集</param>
        /// <returns></returns>
        public static Assembly ConvertToAssembly(this string sourceCode, string[] assemblyNames)
        {
            CSharpCodeProvider ccp = new CSharpCodeProvider();
            CompilerParameters param = new CompilerParameters(assemblyNames);
            CompilerResults cr = ccp.CompileAssemblyFromSource(param, sourceCode);

            return cr.CompiledAssembly;
        }

        /// <summary>
        /// 获取一个值，通过该值指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        /// <param name="value">要测试的字符串。</param>
        /// <returns>如果
        /// <paramref name="value"/>参数为 <c>null</c> 或 <seealso cref="System.String.Empty"/>，或者如果
        /// <paramref name="value"/>仅由空白字符组成，则为 <c>true</c> 。</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        #region String.Format

        /// <summary>
        /// 将指定字符串中的一个或多个格式项替换为指定对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="arg0">要设置格式的对象。</param>
        /// <returns>format 的副本，其中的任何格式项均替换为 arg0 的字符串表示形式。</returns>
        public static string With(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为两个指定对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="arg0">要设置格式的第一个对象。</param>
        /// <param name="arg1">要设置格式的第二个对象。</param>
        /// <returns>format 的副本，其中的格式项替换为 arg0 和 arg1 的字符串表示形式。</returns>
        public static string With(this string format, object arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为三个指定对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="arg0">要设置格式的第一个对象。</param>
        /// <param name="arg1">要设置格式的第二个对象。</param>
        /// <param name="arg2">要设置格式的第三个对象。</param>
        /// <returns>format 的副本，其中的格式项已替换为 arg0、arg1 和 arg2 的字符串表示形式。</returns>
        public static string With(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>format 的副本，其中的格式项已替换为 args 中相应对象的字符串表示形式。</returns>
        public static string With(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        #endregion String.Format

        #region * 常量（全角半角转换）

        // 最大的有效全角英文字符转换成int型数据的值
        private const int MaxSBCCaseToInt = 65374;

        // 最小的有效全角英文字符转换成int型数据的值
        private const int MinSBCCaseToInt = 65281;

        // 最大的有效半角英文字符转换成int型数据的值
        private const int MaxDBCCaseToInt = 126;

        // 最小的有效半角英文字符转换成int型数据的值
        private const int MinDBCCaseToInt = 33;

        private const int SBCBlankToInt = 12288;
        private const int DBCBlankToInt = 32;

        // 对应的全角和半角的差
        private const int Margin = 65248;

        /// <summary>
        /// * 全角字符串转半角
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string ToDBC(this string sText)
        {
            char[] res = sText.ToCharArray();

            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == 12288)
                {
                    res[i] = Convert.ToChar(32);
                    continue;
                }

                if (res[i] >= MinSBCCaseToInt && res[i] <= MaxSBCCaseToInt)
                {
                    res[i] = Convert.ToChar(res[i] - Margin);
                }
            }

            return new string(res);
        }

        #endregion * 常量（全角半角转换）
    }
}