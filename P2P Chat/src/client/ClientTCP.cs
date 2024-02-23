using P2P_Chat.src.conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat.src.client
{
    public static class ClientTCP
    {
        public static void SendTcpRequest(string ipAddress, int tcpPort, string tcpMessage)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(ipAddress, tcpPort);

                byte[] data = Encoding.UTF8.GetBytes(tcpMessage);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                // Čekání na odpověď
                byte[] responseData = new byte[1024];
                int bytesRead = stream.Read(responseData, 0, responseData.Length);
                string response = Encoding.UTF8.GetString(responseData, 0, bytesRead);

                Console.WriteLine("TCP response: " + response);

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending TCP request: " + ex.Message);
            }
        }

        public static void HandleTcpRequests()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse(ConfigHandler.Config.LocalAddr);
                server = new TcpListener(localAddr, Peer.TcpPort);
                server.Start();

                // Čekání na spojení od klienta
                while (true)
                {
                    Console.WriteLine("Waiting for TCP connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("TCP connection accepted.");

                    // Čtení zprávy od klienta
                    NetworkStream stream = client.GetStream();
                    byte[] data = new byte[1024];
                    int bytesRead = stream.Read(data, 0, data.Length);
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    Console.WriteLine("TCP request received: " + message);

                    // Vytvoření odpovědi
                    string responseMessage = "Milošovo TCP funguje!";
                    byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);

                    // Odeslání odpovědi klientovi
                    stream.Write(responseData, 0, responseData.Length);
                    Console.WriteLine("TCP response sent.");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling TCP requests: " + ex.Message);
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}
