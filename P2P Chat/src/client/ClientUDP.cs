﻿using Newtonsoft.Json;
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
    /// <summary>
    /// Třída ClientUDP poskytuje metody pro naslouchání a broadcastování UDP zpráv.
    /// </summary>
    public static class ClientUDP
    {
        public static UdpClient udpClient = Peer.udpClient;
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);
        /// <summary>
        /// Metoda pro naslouchání příchozím UDP zprávám a zpracování odpovědí.
        /// </summary>
        public static void Listener()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(data);
                    string responseMessage = ProcessReceivedMessage(message);

                    if(responseMessage == "TcpStart")
                    {
                        string clientIpAddress = remoteEndPoint.Address.ToString();
                        int clientPort = remoteEndPoint.Port;
                        string tcpMessage = "Hello from Miloš TCP!";

                        ClientTCP.SendTcpRequest(clientIpAddress, clientPort, tcpMessage);
                    }
                    else if (!string.IsNullOrEmpty(responseMessage))
                    {
                        byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                        udpClient.Send(responseData, responseData.Length, remoteEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
        /// <summary>
        /// Metoda pro broadcastování UDP zpráv.
        /// </summary>
        public static void Broadcast()
        {
            while (true)
            {
                try
                {
                    string udpMessage = $"{{\"command\":\"hello\",\"peer_id\":\"{Peer.Id}\"}}";
                    Console.WriteLine("Q: " + udpMessage);
                    byte[] data = Encoding.UTF8.GetBytes(udpMessage);
                    udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, Peer.UdpPort));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
                Thread.Sleep(5000);
            }
        }
        /// <summary>
        /// Metoda pro zpracování přijaté UDP zprávy a vygenerování odpovědi.
        /// </summary>
        /// <param name="receivedMessage">Přijatá zpráva.</param>
        /// <returns>Odpověď na zprávu.</returns>
        public static string ProcessReceivedMessage(string receivedMessage)
        {
            string responseMessage = string.Empty;
            if (receivedMessage != null)
            {
                if (receivedMessage.Contains("\"command\":\"hello\""))
                {
                    responseMessage = $"{{ \"status\":\"ok\", \"peer_id\": \"{Peer.Id}\" }}";
                }
                else if (receivedMessage.Contains("\"status\":\"ok\""))
                {
                    responseMessage = "TcpStart";
                }
            }
            return responseMessage; // Pokud nechcete odpovídat na tento konkrétní typ zprávy, vrátíte null
        }
    }
}
