using Brandy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Test
{
    internal class Program
    {
        static BrandyServer brandyServer = new BrandyServer();
        static void Main(string[] args)
        {
            Console.WriteLine("Brandy Framework Server starting....");
            brandyServer.BrandyClientConnected += Server_BrandyClientConnected;
            brandyServer.BrandyClientDisconnected += Server_BrandyClientDisconnected;
            brandyServer.Listen(80);
            Console.Read();
        }

        private static void Server_BrandyClientDisconnected(object sender, BrandyClient e)
        {
            Console.WriteLine("[ServerSide] A client disconnected");
        }

        private static void Server_BrandyClientConnected(object sender, BrandyClient e)
        {
            e.DataReceived += BrandyClientDataReceived;
            Console.WriteLine("[ServerSide] A client has connected");
            e.Send(Encoding.UTF8.GetBytes("whats up with you"));

        }

        private static void BrandyClientDataReceived(object sender, byte[] e)
        {
            Console.WriteLine("[ServerSide] Client said: " + Encoding.UTF8.GetString(e));
        }
    }
}
