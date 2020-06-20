using System;
using System.Collections;

namespace qsrv.Security
{
    public sealed class SecurityChallenge
    {
        public const string Algorithm = "sha256";
        public const string Encoding = "base64";
        public readonly int Difficulty;
        public readonly string Nonce;

        private SecurityChallenge(int difficulty)
        {
            Difficulty = difficulty;
            Nonce = SecurityManager.GenerateHid();
        }

        public static SecurityChallenge Create(int difficulty)
        {
            return new SecurityChallenge(difficulty);
        }

        public bool Verify(string solution)
        {
            byte[] bytes = Convert.FromBase64String(solution);
            BitArray bitArray = new BitArray(bytes);
            int lastIndex = bitArray.Length - 1;
            for (int i = lastIndex; i > lastIndex - Difficulty; i--)
            {
                bool bit = bitArray[i];
                if (bit)
                {
                    return false;
                }
            }
            return true;
        }
    }
}