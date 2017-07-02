using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPConnection
{
    public class TCPlient : AbstractTCPConnection
    {
        public TCPlient(int portNumber) : base(portNumber) { }
        public TCPlient(int portNumber, string ipAddress) : base(portNumber, ipAddress) { }

        public override void StartConnection()
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.BeginConnect(IPAddress.Parse(IP), PortNumber, new AsyncCallback(ConnectCallback), mainSocket);
            OutputText = "Client setup complete. Waiting for a server";
        }
    }
}
