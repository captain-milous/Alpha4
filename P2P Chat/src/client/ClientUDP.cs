using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using P2P_Chat.src.conf;

namespace P2P_Chat.src.client
{
    public static class ClientUDP
    {

        private static int Port = 9876;
        private static string Peer_id = "Anonymous";
        private static bool BlockConnection = false;

        private static readonly object lockObj = new object();
        private static Queue<string> receivedMessages = new Queue<string>();

        public static void Setup()
        {
            try
            {
                Port = Int32.Parse(ConfigHandler.Config.PortUDP);
                Peer_id = ConfigHandler.Config.PeerID;
            }
            catch (Exception e)
            {
                Console.WriteLine("Chyba v UDP setup: " + e.Message);
                BlockConnection = true;
            }

        }

        public static void SenderThread()
        {
            int failConn = 0;
            while (!BlockConnection)
            {
                Console.WriteLine();
                try
                {
                    var query = new { command = "hello", peer_id = Peer_id };
                    string jsonQuery = JsonConvert.SerializeObject(query);
                    Console.WriteLine("Q: " + jsonQuery);

                    // Odešleme JSON dotaz pomocí UDP broadcastu
                    using (var client = new UdpClient())
                    {
                        client.EnableBroadcast = true;
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, Port);
                        byte[] bytes = Encoding.ASCII.GetBytes(jsonQuery);
                        client.Send(bytes, bytes.Length, endPoint);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sender error: " + e.Message);
                    failConn++;
                }
                if (failConn < 5)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.WriteLine("Connection Lost");
                    BlockConnection = true;
                }
            }
        }

        public static void ReceiverThread()
        {
            var listener = new UdpClient(Port);

            while (!BlockConnection)
            {
                string command = string.Empty;
                string status = string.Empty;
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = listener.Receive(ref endPoint);
                    string receivedJson = Encoding.ASCII.GetString(receivedBytes);
                    dynamic receivedQuery = JsonConvert.DeserializeObject(receivedJson);
                    //Console.WriteLine(receivedQuery);
                    try
                    {
                        command = (string)receivedQuery.command;
                    }
                    catch { }
                    try
                    {
                        status = (string)receivedQuery.status;
                    }
                    catch { }
                    if (command == "hello")
                    {
                        var response = new { status = "ok", peer_id = Peer_id };
                        string jsonResponse = JsonConvert.SerializeObject(response);
                        listener.EnableBroadcast = true;
                        endPoint = new IPEndPoint(IPAddress.Broadcast, Port);
                        byte[] responseBytes = Encoding.ASCII.GetBytes(jsonResponse);
                        listener.Send(responseBytes, responseBytes.Length, endPoint);
                    }
                    else if (!string.IsNullOrEmpty(status))
                    {
                        if(receivedQuery.peer_id != Peer_id)
                        {
                            AddReceivedMessage("A: " + receivedJson);
                        }
                    }
                }
                catch (Exception e)
                {
                    string message = e.Message;
                    if (!message.Contains("An existing connection was forcibly closed by the remote host."))
                    {
                        Console.WriteLine("Receiver error: " + e.Message);
                        BlockConnection = true;
                    }
                }

            }
        }

        // Metoda pro přidání přijaté zprávy do fronty
        public static void AddReceivedMessage(string message)
        {
            lock (lockObj)
            {
                receivedMessages.Enqueue(message);
            }
        }

        // Metoda pro získání a odebrání další přijaté zprávy z fronty
        public static string GetNextReceivedMessage()
        {
            lock (lockObj)
            {
                if (receivedMessages.Count > 0)
                {
                    return receivedMessages.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
