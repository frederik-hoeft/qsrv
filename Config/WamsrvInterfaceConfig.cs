namespace qsrv.Config
{
    public class WamsrvInterfaceConfig
    {
        public readonly string DatabaseServerIp;
        public readonly int DatabaseServerPort;

        public WamsrvInterfaceConfig(string databaseServerIp, int databaseServerPort)
        {
            DatabaseServerIp = databaseServerIp;
            DatabaseServerPort = databaseServerPort;
        }
    }
}