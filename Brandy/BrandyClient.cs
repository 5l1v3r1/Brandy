using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brandy
{
    public class BrandyClient
    {

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        private bool _disconnected;

        private TcpClient _client;
        private NetworkStream _stream;
        public BrandyServer owner;

        public BrandyClient(TcpClient source)
        {
            _client = source;
            // server is responsible for setupstreams call 
        }
        public BrandyClient()
        {
            _client = new TcpClient();
        }
        public void Connect(string host, int port)
        {
         
            _client.BeginConnect(host, port, new AsyncCallback(ConnectCallback), this);

        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            BrandyClient client = (BrandyClient)ar.AsyncState;
            client._client.EndConnect(ar);
            if (client._client.Connected)
            {
                client.InitializeReceiveThread();
                client.Connected?.Invoke(client, null);
            }
        }
        public void InitializeReceiveThread()
        {
            _disconnected = false;
            _stream = _client.GetStream();
            Thread receiveThread = new Thread(Receive);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        private void Receive()
        {
            while (!_disconnected )
            {
                try
                {
                    int totalHeaderRead = 0;
                    byte[] buffer = new byte[8];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    totalHeaderRead += bytesRead;
                    while (totalHeaderRead < 8)
                    {
                        bytesRead = _stream.Read(buffer, totalHeaderRead, 8 - totalHeaderRead);
                        totalHeaderRead += bytesRead;
                    }
                    if (totalHeaderRead == 8)
                    {
                        int totalRead = 0;
                        long amountToReceive = buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24 |
                                               buffer[4] << 32 | buffer[5] << 40 | buffer[6] << 48 | buffer[7] << 56;
                        byte[] dataToRead = new byte[amountToReceive];
                        bytesRead = _stream.Read(dataToRead, 0, dataToRead.Length);
                        totalRead += bytesRead;
                        while (totalRead < amountToReceive)
                        {
                            bytesRead = _stream.Read(dataToRead, totalRead, (int)amountToReceive - totalRead);
                            totalRead += bytesRead;
                        }

                        if (totalRead == amountToReceive)
                        {
                            DataReceived?.Invoke(this, dataToRead);
                        }

                        //incomingData.Enqueue(dataToRead);
                    }
                }
                catch (IOException e)
                {
                    // client likely disconnected
                    _disconnected = true;
                    Disconnected?.Invoke(this, null);
                }
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                byte[] totalData = new byte[8];
                Marshal.WriteInt64(totalData, 0, (long)data.Length);
                for (int i = 0; i < totalData.Length; i++)
                {
                    _stream.WriteByte((byte)((totalData[i] >> (i * 8)) & 0xff));
                }
                _stream.Write(data, 0, data.Length);
                _stream.Flush();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
