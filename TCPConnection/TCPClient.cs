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
        public TCPlient(int portNumber) : base(portNumber)
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public TCPlient(int portNumber, string ipAddress) : base(portNumber, ipAddress)
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override void StartConnection()
        {
            try
            {
                //Change it
                mainSocket.BeginConnect(IPAddress.Parse(IP), PortNumber, new AsyncCallback(ConnectCallback), mainSocket);
                //mainSocket.BeginConnect(IPAddress.Loopback, PortNumber, new AsyncCallback(ConnectCallback), mainSocket);
            }
            catch (Exception)
            {
                CloseConnection("Couldn't  connect to the server (begin)");
                return;
            }
            OutputText = "Client setup complete. Waiting for a server";
        }

    }
}
