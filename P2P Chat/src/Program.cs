﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using P2P_Chat.src.client;
using P2P_Chat.src.conf;

namespace P2P_Chat.src
{
    public class Program
    {

        static void Main(string[] args)
        {
            ConfigHandler.Load();
            ClientUDP.Setup();

            // Spustíme vlákno pro periodické odesílání JSON dotazu
            Thread senderThread = new Thread(ClientUDP.SenderThread);
            senderThread.Start();

            // Spustíme vlákno pro příjem JSON dotazů
            Thread receiverThread = new Thread(ClientUDP.ReceiverThread);
            receiverThread.Start();
        }

        
    }
}