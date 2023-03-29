using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Brandy
{
    internal class ServerState
    {
        public BrandyServer brandyServerInstance;
        public TcpListener listener;
    }
}
