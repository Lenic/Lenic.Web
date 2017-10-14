using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lenic.Framework.Common.Security
{
    /// <summary>
    /// MD5 加密算法帮助类
    /// </summary>
    public static class MD5Helper
    {
        #region Private Fields

        private static MD5CryptoServiceProvider _provider = new MD5CryptoServiceProvider();

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// 使用 MD5 加密算法加密数据。
        /// </summary>
        /// <param name="input">待加密的原始数据。</param>
        /// <returns>MD5 算法加密后的数据。</returns>
        public static string MD5(this string input)
        {
            var data = Encoding.Default.GetBytes(input);

            return _provider.ComputeHash(data)
                            .Aggregate(new StringBuilder(), (x, y) => x.Append(y.ToString("x2")))
                            .ToString();
        }

        #endregion Entrance
    }
}