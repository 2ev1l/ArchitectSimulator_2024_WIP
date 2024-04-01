using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Universal
{
    public static class Cryptography
    {
        #region fields & properties
        private static readonly byte[] iv = new byte[16] { 0x0A, 0xF1, 0x2D, 0x2B, 0xC4, 0xE3, 0xF1, 0x2F, 0xA2, 0x18, 0x25, 0x13, 0x2A, 0xBE, 0xFA, 0xAE };
        private static readonly string password = "J92AL%!DGawr(L7246HSz%^AKnl%*)09";
        #endregion fields & properties
        
        #region methods
        public static string Encrypt(string message)
        {
            System.Security.Cryptography.SHA256 mySHA256 = System.Security.Cryptography.SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
            return EncryptString(message, key, iv);
        }
        public static string Decrypt(string encrypted)
        {
            System.Security.Cryptography.SHA256 mySHA256 = System.Security.Cryptography.SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
            return DecryptString(encrypted, key, iv);
        }
        private static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }
        private static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
            #endregion methods
        }
    }
}