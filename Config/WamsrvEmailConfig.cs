namespace qsrv.Config
{
    public class WamsrvEmailConfig
    {
        public readonly string EmailDisplayName;
        public readonly string EmailAddress;
        public readonly string EmailPassword;
        public readonly string SmptServer;
        public readonly int SmptServerPort;

        public WamsrvEmailConfig(string emailDisplayName, string emailAddress, string emailPassword, string smptServer, int smptServerPort)
        {
            EmailDisplayName = emailDisplayName;
            EmailAddress = emailAddress;
            EmailPassword = emailPassword;
            SmptServer = smptServer;
            SmptServerPort = smptServerPort;
        }
    }
}