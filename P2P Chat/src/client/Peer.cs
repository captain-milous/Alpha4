using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using P2P_Chat.src.conf;
using Newtonsoft.Json;

namespace P2P_Chat.src.client
{
    public static class Peer
    {
        private static Config Config = ConfigHandler.Config;
        public static string Id = "tesar-peer1";
        public static int UdpPort = 9876;
        public static int TcpPort = 9876;
        public static UdpClient udpClient;

        public static void Setup()
        {
            try
            {
                Id = Config.PeerID;
                UdpPort = Int32.Parse(Config.PortUDP);
                TcpPort = Int32.Parse(Config.PortTCP);

                udpClient = new UdpClient(UdpPort);
                Thread ListenerUdp = new Thread(UdpListener);
                ListenerUdp.Start();

                Thread BroadcastUdp = new Thread(UdpBroadcast);
                BroadcastUdp.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
