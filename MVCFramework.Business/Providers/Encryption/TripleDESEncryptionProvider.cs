using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MVCFramework.Business.Providers.Encryption
{
    public class TripleDESEncryptionProvider : EncryptionProviderBase
    {
        public override string Encrypt(string data)
        {
            byte[] encrypted;

            CheckInitialization();

            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = _key;
                tdes.IV = Encoding.UTF8.GetBytes(_salt).Take(tdes.BlockSize / 8).ToArray();

                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                encrypted = Transform(dataBytes, tdes.CreateEncryptor());
            }

            return Convert.ToBase64String(encrypted);
        }

        public override string Decrypt(string data)
        {
            byte[] decrypted;

            CheckInitialization();

            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = _key;
                tdes.IV = Encoding.UTF8.GetBytes(_salt).Take(tdes.BlockSize / 8).ToArray();
                byte[] dataBytes = Convert.FromBase64String(data);

                decrypted = Transform(dataBytes, tdes.CreateDecryptor());
            }

            return Encoding.UTF8.GetString(decrypted);
        }

        public override void InitializeProvider(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, saltBytes);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.IV = saltBytes.Take(tdes.BlockSize / 8).ToArray();

            byte[] key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);

            _key = key;

            base.InitializeProvider(password, salt);
        }
    }
}
