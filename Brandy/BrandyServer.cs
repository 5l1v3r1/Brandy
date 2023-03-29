using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Brandy
{
    public class BrandyServer
    {
        private List<TcpListener> listeners = new List<TcpListener>();
        private List<BrandyClient> clients = new List<BrandyClient >();
        public event EventHandler<BrandyClient> BrandyClientConnected;
        public event EventHandler<BrandyClient > BrandyClientDisconnected;
        public void Listen(params int[] port)
        {
            for (int i = 0; i < port.Length; i++)
            {
                TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, port[i]));
                listeners.Add(listener);
                listener.Start();
                ServerState state = new ServerState();
                state.brandyServerInstance = this;
                state.listener = listener;
                listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), state);
            }
        }
        private static void AcceptTcpClientCallback(IAsyncResult ar)
        {
            ServerState state = (ServerState)ar.AsyncState;
            TcpClient tcpConnection = state.listener.EndAcceptTcpClient(ar);
            BrandyClient brandyClient = new BrandyClient(tcpConnection);
            brandyClient.owner = state.brandyServerInstance;
            brandyClient.Disconnected += Wrapper_Disconnected;
            lock (state.brandyServerInstance.clients)
            {
                state.brandyServerInstance.clients.Add(brandyClient);
            }
            brandyClient.InitializeReceiveThread();
            state.brandyServerInstance.BrandyClientConnected?.Invoke(state.listener, brandyClient);
            state.listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), state.listener);
        }

        private static void Wrapper_Disconnected(object sender, EventArgs e)
        {
            BrandyClient brandyClient = (BrandyClient)sender;
            brandyClient.owner.BrandyClientDisconnected?.Invoke(sender, null);
            lock(brandyClient.owner.clients )
            {
                brandyClient.owner.clients.Remove((BrandyClient)sender);
            }
        }
    }
}
