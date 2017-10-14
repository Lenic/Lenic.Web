namespace Lenic.Framework.Common.Security
{
    /// <summary>
    /// Rsa 加密算法运算过程中用到的密钥
    /// </summary>
    public class RsaKey
    {
        /// <summary>
        /// 获取加密过程中用到的私有密钥。
        /// </summary>
        public string PrivateKey { get; private set; }

        /// <summary>
        /// 获取解密过程中用到的公有密钥。
        /// </summary>
        public string PublicKey { get; private set; }

        /// <summary>
        /// 初始化新建一个 <see cref="RsaKey"/> 类的实例对象。
        /// </summary>
        /// <param name="privateKey">加密过程中用到的私有密钥。</param>
        /// <param name="publicKey">解密过程中用到的公有密钥。</param>
        public RsaKey(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = privateKey;
        }
    }
}