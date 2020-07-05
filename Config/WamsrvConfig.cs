namespace qsrv.Config
{
    public class WamsrvConfig
    {
        public readonly bool DebuggingEnabled;
        public readonly bool AdvancedErrorChecking;
        public readonly int LocalPort;
        public readonly string PfxCertificatePath;
        public readonly string PfxPassword;
        public readonly bool SuppressCertificateErrors;
        public readonly WamsrvInterfaceConfig WamsrvInterfaceConfig;
        public readonly WamsrvSecurityConfig WamsrvSecurityConfig;
        public readonly WamsrvDevelopmentConfig WamsrvDevelopmentConfig;

        public WamsrvConfig(bool debuggingEnabled, bool advancedErrorChecking, int localPort, string pfxCertificatePath, string pfxPassword, bool suppressCertificateErrors, WamsrvInterfaceConfig wamsrvInterfaceConfig, WamsrvSecurityConfig wamsrvSecurityConfig, WamsrvDevelopmentConfig wamsrvDevelopmentConfig)
        {
            DebuggingEnabled = debuggingEnabled;
            AdvancedErrorChecking = advancedErrorChecking;
            LocalPort = localPort;
            PfxCertificatePath = pfxCertificatePath;
            PfxPassword = pfxPassword;
            SuppressCertificateErrors = suppressCertificateErrors;
            WamsrvInterfaceConfig = wamsrvInterfaceConfig;
            WamsrvSecurityConfig = wamsrvSecurityConfig;
            WamsrvDevelopmentConfig = wamsrvDevelopmentConfig;
        }
    }
}