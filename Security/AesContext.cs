using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace qsrv.Security
{
    public class AesContext
    {
        private readonly string password;

        public AesContext(string userid)
        {
            password = SecurityManager.DeriveUserSecret(userid);
        }

        public string EncryptOrDefault(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return AESEncrypt(text, password);
        }

        public string DecryptOrDefault(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return AESDecrypt(text, password);
        }

        // using AES with:
        // Key hash algorithm: SHA-256
        // Key Size: 256 Bit
        // Block Size: 128 Bit
        // Input Vector (IV): 128 Bit
        // Mode of Operation: Cipher-Block Chaining (CBC)
        /// <summary>
        /// Encrypts a plain text with a password using AES-256 CBC with SHA-256.
        /// </summary>
        /// <param name="plainText">The text to be encrypted.</param>
        /// <param name="password">The password to encrypt the text with.</param>
        /// <returns>The encrypted text.</returns>
        private static string AESEncrypt(string plainText, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using AesCng AES = new AesCng
                {
                    KeySize = 256,
                    Key = hashedPasswordBytes
                };
                AES.IV = SecurityManager.GetRandomBytes(AES.BlockSize / 8);
                AES.Mode = CipherMode.CBC;

                using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.Close();
                }
                encryptedBytes = new byte[AES.IV.Length + ms.ToArray().Length];
                AES.IV.CopyTo(encryptedBytes, 0);
                ms.ToArray().CopyTo(encryptedBytes, AES.IV.Length);
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts a cipher text with a password using AES-256 CBC with SHA-256.
        /// </summary>
        /// <param name="cipherText">The cipher text to be decrypted.</param>
        /// <param name="password">The password to decrypt the cipher text with.</param>
        /// <returns>The decrypted text.</returns>
        private static string AESDecrypt(string cipherText, string password)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }
            byte[] iv = new byte[16];
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            Array.Copy(cipherBytes, iv, 16);
            byte[] decryptedBytes = null;
            using (AesCng AES = new AesCng())
            {
                AES.IV = iv;
                AES.KeySize = 256;
                AES.Mode = CipherMode.CBC;
                AES.Key = hashedPasswordBytes;
                using ICryptoTransform decryptor = AES.CreateDecryptor();
                using MemoryStream msDecrypted = new MemoryStream();
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypted, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(cipherBytes, 16, cipherBytes.Length - 16);
                    csDecrypt.Close();
                }
                decryptedBytes = msDecrypted.ToArray();
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}