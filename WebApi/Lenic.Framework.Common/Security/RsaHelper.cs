using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lenic.Framework.Common.Security
{
    /// <summary>
    /// RSA 加密算法帮助类
    /// </summary>
    public sealed class RsaHelper
    {
        #region Private Fields

        private RSACryptoServiceProvider _provider;
        private int _maxEncryptBlockSize;
        private int _maxDecryptBlockSize;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// 获取 RSA 加解密过程中用到的密钥。
        /// </summary>
        public string Key { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// 初始化新建一个 <see cref="RsaHelper"/> 类的实例对象。
        /// </summary>
        /// <param name="key">RSA 加解密过程中用到的密钥。</param>
        /// <exception cref="System.ArgumentNullException">[RsaHelper].[ctor].key</exception>
        public RsaHelper(string key)
        {
            if (key == null)
                throw new ArgumentNullException("[RsaHelper].[ctor].key");

            Key = key;
            _provider = new RSACryptoServiceProvider();
            _provider.FromXmlString(key);
            _maxEncryptBlockSize = _provider.KeySize / 8 - 11;
            _maxDecryptBlockSize = _provider.KeySize / 8;
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// 使用 RSA 加密算法加密数据。
        /// </summary>
        /// <param name="source">待加密的原始数据。</param>
        /// <returns>RSA 加密后的数据。</returns>
        public string Encrypt(string source)
        {
            var data = Encoding.Unicode.GetBytes(source);
            if (data.Length <= _maxEncryptBlockSize)
                return Convert.ToBase64String(_provider.Encrypt(data, false));

            using (MemoryStream sourceStream = new MemoryStream(data))
            using (MemoryStream targetStream = new MemoryStream())
            {
                var buffer = new byte[_maxEncryptBlockSize];
                int blockSize = sourceStream.Read(buffer, 0, _maxEncryptBlockSize);

                while (blockSize > 0)
                {
                    var toEncrypt = new byte[blockSize];
                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);

                    var cryptograph = _provider.Encrypt(toEncrypt, false);
                    targetStream.Write(cryptograph, 0, cryptograph.Length);

                    blockSize = sourceStream.Read(buffer, 0, _maxEncryptBlockSize);
                }

                return Convert.ToBase64String(targetStream.ToArray(), Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// 使用 RSA 加密算法解密数据。
        /// </summary>
        /// <param name="source">待解密的数据。</param>
        /// <returns>解密后的原始数据。</returns>
        public string Decrypt(string source)
        {
            var data = Convert.FromBase64String(source);
            if (data.Length <= _maxDecryptBlockSize)
                return Encoding.Unicode.GetString(_provider.Decrypt(data, false));

            using (MemoryStream sourceStream = new MemoryStream(data))
            using (MemoryStream targetStream = new MemoryStream())
            {
                var buffer = new Byte[_maxDecryptBlockSize];
                int blockSize = sourceStream.Read(buffer, 0, _maxDecryptBlockSize);

                while (blockSize > 0)
                {
                    var toDecrypt = new byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);

                    var plaintext = _provider.Decrypt(toDecrypt, false);
                    targetStream.Write(plaintext, 0, plaintext.Length);

                    blockSize = sourceStream.Read(buffer, 0, _maxDecryptBlockSize);
                }

                return Encoding.Unicode.GetString(targetStream.ToArray());
            }
        }

        /// <summary>
        /// 创建一个新的 RSA 算法加密和解密过程中用到的密钥。
        /// </summary>
        /// <returns></returns>
        public static RsaKey CreateNewKey()
        {
            var provider = new RSACryptoServiceProvider();
            return new RsaKey(provider.ToXmlString(true), provider.ToXmlString(false));
        }

        #endregion Business Methods
    }
}