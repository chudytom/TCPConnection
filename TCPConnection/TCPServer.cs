using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPConnection
{
    public class TCPServer : AbstractTCPConnection
    {
        private List<Socket> clientSockets = new List<Socket>();
        public TCPServer(int portNumber) : base(portNumber)
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public TCPServer(int portNumber, string ipAddress) : base(portNumber, ipAddress)
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override void StartConnection()
        {
            mainSocket.Bind(new IPEndPoint(IPAddress.Any, PortNumber));
            mainSocket.Listen(0);
            mainSocket.BeginAccept(AcceptCallback, null);
            //ReceiveResponse();
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            Socket currentSocket;
            try
            {
                currentSocket = mainSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            clientSockets.Add(currentSocket);
            IsConnected = true;
            ReceiveResponse();
            Console.WriteLine("Client connected, waiting for request");
            mainSocket.BeginAccept(AcceptCallback, null);
        }
    }
}
