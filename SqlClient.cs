using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using washared;

namespace qsrv
{
    public class SqlClient : DisposableNetworkInterface
    {
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None || MainServer.Config.SuppressCertificateErrors)
            {
                return true;
            }

            Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

        public SqlClient(string ip, int port) : base(ip, port)
        {
            Network = new Network(this);
            SslStream = new SslStream(NetworkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
            X509Certificate2Collection certificates = new X509Certificate2Collection
            {
                MainServer.ServerCertificate
            };
            SslStream.AuthenticateAsClient(string.Empty, certificates, SslProtocols.None, true);
        }
    }
}