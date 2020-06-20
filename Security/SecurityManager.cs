using Scrypt;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace qsrv.Security
{
    public static class SecurityManager
    {
        // TODO: Catch encrypt / decrypt errors
        private readonly static RNGCryptoServiceProvider rngCryptoService = new RNGCryptoServiceProvider();

        #region AES

        

        /// <summary>
        /// Generates cryptographically secure random bytes.
        /// </summary>
        /// <param name="saltLength">The number of bytes to be generated.</param>
        /// <returns>Cryptographically secure random bytes.</returns>
        public static byte[] GetRandomBytes(int saltLength)
        {
            byte[] randomBytes = new byte[saltLength];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            return randomBytes;
        }

        #endregion AES

        #region SCrypt

        /// <summary>
        /// Creates the scrypt hash of a password and salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The hex encoded salted scrypt hash of the password.</returns>
        public static string ScryptHash(string password)
        {
            ScryptEncoder scrypt = new ScryptEncoder(65536, 8, 1, rngCryptoService);
            return scrypt.Encode(password);
        }

        public static bool ScryptCheck(string password, string hash)
        {
            ScryptEncoder scrypt = new ScryptEncoder(65536, 8, 1, rngCryptoService);
            return scrypt.Compare(password, hash);
        }

        #endregion SCrypt

        public static string GenerateSecurityToken()
        {
            using SHA512Managed hashFunction = new SHA512Managed();
            byte[] plainBytes = GetRandomBytes(2048);
            byte[] token = hashFunction.ComputeHash(plainBytes);
            return Convert.ToBase64String(token);
        }

        public unsafe static string GenerateSecurityCode()
        {
            int length = MainServer.Config.WamsrvSecurityConfig.TwoFactorCodeLength * sizeof(uint);
            byte[] buffer = new byte[length];
            rngCryptoService.GetBytes(buffer);
            uint[] ints = new uint[length];
            fixed (byte* b = buffer)
            {
                for (int i = 0; i < length; i++)
                {
                    ints[i] = *(uint*)(b + (i * sizeof(uint)));
                }
            }
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                uint result = ints[i] % 10u;
                builder.Append(result.ToString());
            }
            return builder.ToString();
        }

        public static string GenerateHid()
        {
            using SHA256Managed hashFunction = new SHA256Managed();
            byte[] plainBytes = GetRandomBytes(2048);
            byte[] passwordBytes = hashFunction.ComputeHash(plainBytes);
            return Convert.ToBase64String(passwordBytes);
        }

        public static string DeriveUserSecret(string userid)
        {
            using SHA256Managed hashFunction = new SHA256Managed();
            byte[] plainBytes = Encoding.UTF8.GetBytes(userid + MainServer.Config.WamsrvSecurityConfig.ServerSecret);
            byte[] passwordBytes = hashFunction.ComputeHash(plainBytes);
            return Convert.ToBase64String(passwordBytes);
        }
    }
}