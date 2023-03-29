using Brandy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Test
{
    internal class Program
    {
        static BrandyClient brandyClient = new BrandyClient();

        static void Main(string[] args)
        {
            brandyClient.Connected += Client_Connected;
            brandyClient.DataReceived += Client_DataReceived;
            brandyClient.Disconnected += Client_Disconnected;
            brandyClient.Connect("127.0.0.1", 80);
            Console.Read();

        }

        private static void Client_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }

        private static void Client_DataReceived(object sender, byte[] e)
        {
            Console.WriteLine("Server said: " + Encoding.UTF8.GetString(e));
        }

        private static void Client_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
            brandyClient.Send(Encoding.UTF8.GetBytes("What's up serva "));
        }
    }
}
