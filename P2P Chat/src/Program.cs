using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using P2P_Chat.src.client;
using P2P_Chat.src.conf;

namespace P2P_Chat.src
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConfigHandler.Load();
            Peer.Setup();
        }
          
    }
}