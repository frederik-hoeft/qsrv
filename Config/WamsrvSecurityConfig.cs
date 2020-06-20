namespace qsrv.Config
{
    public class WamsrvSecurityConfig
    {
        public readonly int ScryptIterationCount;
        public readonly int ScryptBlockSize;
        public readonly int ScryptThreadCount;
        public readonly int TwoFactorExpirationTime;
        public readonly int TwoFactorCodeLength;
        public readonly int SecurityTokenExpirationTime;
        public readonly string ServerSecret;

        public WamsrvSecurityConfig(int scryptIterationCount, int scryptBlockSize, int scryptThreadCount, int twoFactorExpirationTime, int twoFactorCodeLength, int securityTokenExpirationTime, string serverSecret)
        {
            ScryptIterationCount = scryptIterationCount;
            ScryptBlockSize = scryptBlockSize;
            ScryptThreadCount = scryptThreadCount;
            ServerSecret = serverSecret;
            TwoFactorExpirationTime = twoFactorExpirationTime;
            SecurityTokenExpirationTime = securityTokenExpirationTime;
            TwoFactorCodeLength = twoFactorCodeLength;
        }
    }
}