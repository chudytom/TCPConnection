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
        public TCPServer(int portNumber) : base(portNumber) { }
        public TCPServer(int portNumber, string ipAddress) : base(portNumber, ipAddress) { }

        public override void StartConnection()
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Bind(new IPEndPoint(IPAddress.Any, PortNumber));
            mainSocket.Listen(0);
            mainSocket.BeginAccept(AcceptCallback, null);
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
            ReceiveResponse(currentSocket);
            Console.WriteLine("Client connected, waiting for request");
            mainSocket.BeginAccept(AcceptCallback, null);
        }
    }
}
