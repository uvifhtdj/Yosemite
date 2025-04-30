using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace wrapper
{
    public static class SSLOrTLSUtil
    {
        public static void FixSSLTLSError()
        {
            ServicePointManager.ServerCertificateValidationCallback = (object obj, X509Certificate x509, X509Chain chain, SslPolicyErrors errors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public static void ResetSecurityProtocol()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
        }
    }
}
