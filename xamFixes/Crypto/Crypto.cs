using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace xamFixes.Crypto
{
    public static class FixesCrypto
    {
        public static KeyPair GenerateKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                return new KeyPair() { PublicKey = rsa.ToXmlString(false), PrivateKey = rsa.ToXmlString(true) };
        }

        public static byte[] EncryptText(string publicKey, string text)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(text);

            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }

            return encryptedData;
        }

        public static string DecryptData(string privateKey, byte[] message)
        {
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(message, false);
            }

            UnicodeEncoding byteConverter = new UnicodeEncoding();
            return byteConverter.GetString(decryptedData);
        }
    }
}