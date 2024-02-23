using System.Net.Sockets;
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
            Peer.Setup();
        }
          
    }
}