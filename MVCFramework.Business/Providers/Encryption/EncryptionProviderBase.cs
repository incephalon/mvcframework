using System;
using System.IO;
using System.Security.Cryptography;

namespace MVCFramework.Business.Providers.Encryption
{
    public abstract class EncryptionProviderBase
    {
        private bool _initialized = false;
        protected byte[] _key = null;
        protected string _password = null;
        protected string _salt = null;


        public virtual void InitializeProvider(byte[] key)
        {
            _key = key;
            _initialized = true;
        }

        public virtual void InitializeProvider(string password, string salt)
        {
            _password = password;
            _salt = salt;

            _initialized = true;
        }

        public abstract string Encrypt(string data);
        public abstract string Decrypt(string data);

        protected void CheckInitialization()
        {
            if (!_initialized)
                throw new NotImplementedException("Provider is not initialized!");
        }

        protected byte[] Transform(byte[] data, ICryptoTransform cryptoTransform)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    result = ms.ToArray();

                    cs.Close();
                    ms.Close();
                }
            }

            return result;
        }
    }
}
