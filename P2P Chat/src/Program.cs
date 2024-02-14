using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace P2P_Chat.src
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Spustíme vlákno pro periodické odesílání JSON dotazu
            Thread senderThread = new Thread(SenderThread);
            senderThread.Start();

            // Spustíme vlákno pro příjem JSON dotazů
            Thread receiverThread = new Thread(ReceiverThread);
            receiverThread.Start();
        }

        static void SenderThread()
        {
            while (true)
            {
                // Vytvoříme JSON dotaz
                var query = new { message = "Hello from peer" };
                string jsonQuery = JsonConvert.SerializeObject(query);

                Console.WriteLine("Q: " + jsonQuery);

                // Odešleme JSON dotaz pomocí UDP broadcastu
                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 12345);
                    byte[] bytes = Encoding.ASCII.GetBytes(jsonQuery);
                    client.Send(bytes, bytes.Length, endPoint);
                }

                // Počkáme 5 sekund
                Thread.Sleep(5000);
            }
        }

        static void ReceiverThread()
        {
            // Vytvoříme UDP listener na portu 9876
            var listener = new UdpClient(12345);

            while (true)
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
        }
    }
}