using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ClassLibraryA;

namespace DemoUseLowerLib
{
    class Program
    {
        public static void Main(string[] args)
        {
            string serverCertificateName = null;
            string serverName = null;
            if (args == null || args.Length < 1)
            {
                serverName = Environment.MachineName;
            }
            else
                serverName = args[0];

            // User can specify the machine name and server name.
            // Server name must match the name on the server's certificate.
            if (args.Length < 2)
            {
                serverCertificateName = serverName;
            }
            else
            {
                serverCertificateName = args[1];
            }

            // Create a TCP/IP client socket.
            // machineName is the host running the server application.
            TcpClient client = new TcpClient(serverName, 8080);
            Console.WriteLine("Client connected.");
            var sslOverStream = new SslOverTdsStream(client.GetStream());
            // Create an SSL stream that will close the client's stream.
            SslStream sslStream = new SslStreamWrapper(
                sslOverStream, //client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );

            // The server name must match the name on the server certificate.
            try
            {
                sslStream.AuthenticateAsClient(serverName);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }

            Stream stream = sslStream;
            RunTest(stream);
        }

        public static void RunTest(Stream stream)
        {
            const string aRecord = "('2455cf1b-ebcf-418d-8cce-88e21e1683e3', 'something', 'updated'),";
            string script = "SELECT * FROM (VALUES"
                                    + string.Concat(Enumerable.Repeat(aRecord, 400)).Substring(0, (aRecord.Length * 400) - 1)
                                    + ") tbl_A ([Id], [Name], [State]) <EOF>";
            byte[] message = Encoding.UTF8.GetBytes(script);
            var packet = new Packet(message);
            packet.WriteToStreamAsync(stream);
            stream.Flush();
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}
