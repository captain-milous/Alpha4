using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat.src.client
{
    public static class ClientUDP
    {
        public static void SenderThread()
        {
            while (true)
            {
                Console.WriteLine();
                try
                {
                    // Vytvoříme JSON dotaz
                    var query = new { message = "Hello from peer" };
                    string jsonQuery = JsonConvert.SerializeObject(query);

                    Console.WriteLine("Q: " + jsonQuery);

                    // Odešleme JSON dotaz pomocí UDP broadcastu
                    using (var client = new UdpClient())
                    {
                        client.EnableBroadcast = true;
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 9876);
                        byte[] bytes = Encoding.ASCII.GetBytes(jsonQuery);
                        client.Send(bytes, bytes.Length, endPoint);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sender error: " + e.Message);
                }


                // Počkáme 5 sekund
                Thread.Sleep(5000);
            }
        }

        public static void ReceiverThread()
        {
            // Vytvoříme UDP listener na portu 9876
            var listener = new UdpClient(9876);

            while (true)
            {
                try
                {
                    // Přijmeme zprávu
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = listener.Receive(ref endPoint);
                    string receivedJson = Encoding.ASCII.GetString(receivedBytes);

                    // Deserializujeme přijatý JSON dotaz
                    dynamic receivedQuery = JsonConvert.DeserializeObject(receivedJson);
                    string message = receivedQuery.message;

                    // Vytvoříme JSON odpověď
                    var response = new { responseMessage = "Hi, I received your message: " + message };
                    string jsonResponse = JsonConvert.SerializeObject(response);

                    Console.WriteLine("A: " + jsonResponse);

                    // Odešleme JSON odpověď zpět na stejnou adresu, ze které jsme obdrželi dotaz
                    byte[] responseBytes = Encoding.ASCII.GetBytes(jsonResponse);
                    listener.Send(responseBytes, responseBytes.Length, endPoint);
                }
                catch (Exception e)
                {
                    //Console.WriteLine("Receiver error: " + e.Message);
                }

            }
        }
    }
}
