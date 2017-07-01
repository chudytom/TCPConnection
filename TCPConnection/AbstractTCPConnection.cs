using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPConnection
{
    public abstract class AbstractTCPConnection
    {
        protected Socket mainSocket;
        public bool IsConnected { get; set; }
        public string IP { get; set; } = IPAddress.Loopback.ToString();
        public int PortNumber { get; set; }
        //Decide which properties beneatch should be made available
        public string OutputText { get; set; }
        public string IncomingText { get; set; }
        public string InputText { get; set; }

        //const int PORT = 100;
        protected const int BUFFER_SIZE = 1024;
        protected byte[] incomingBuffer = new byte[BUFFER_SIZE];
        /// <summary>
        /// The default value of IPAdress is IPAddress.Loopback
        /// </summary>
        /// <param name="portNumber"></param>
        public AbstractTCPConnection(int portNumber)
        {
            IsConnected = false;
            this.PortNumber = portNumber;
        }
        public AbstractTCPConnection(int portNumber, string ipAddress) : this(portNumber)
        {
            this.IP = ipAddress;
        }

        abstract public void StartConnection();

        protected void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                mainSocket.EndConnect(AR);
            }
            catch (SocketException)
            {
                OutputText = "Cannot connect to the server (end, SocketException)";
                return;
            }
            catch (ArgumentException)
            {
                OutputText = "Cannot connect to the server(end, ArgumentException)";
                return;
            }
            IsConnected = mainSocket.Connected;
            ReceiveResponse();
            OutputText = "Connected";
        }

        public void SendString(string stringToSend)
        {
            byte[] outgoingBuffer = Encoding.ASCII.GetBytes(stringToSend);
            try
            {
                mainSocket.Send(outgoingBuffer, 0, outgoingBuffer.Length, SocketFlags.None);
            }
            catch (SocketException)
            {
                OutputText = "Unable to send string (SocketException)";
                mainSocket.Close();
                return;
            }
            catch (ObjectDisposedException)
            {
                OutputText = "Unable to send string (ObjectDisposedException)";
                CloseConnection();
                return;
            }
        }

        /// <summary>
        /// BeginReceive with exception handling
        /// </summary>
        protected void ReceiveResponse()
        {
            try
            {
                mainSocket.BeginReceive(incomingBuffer, 0, BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), mainSocket);
                Thread.Sleep(200);
            }
            catch (SocketException)
            {
                CloseConnection("This error");
                //StartConnection();
                Thread.Sleep(3000);
            }
            catch (Exception)
            {
                CloseConnection("Sudden connection lost");
            }
        }

        protected void ReceiveResponse(Socket socket)
        {
            try
            {
                socket.BeginReceive(incomingBuffer, 0, BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                Thread.Sleep(200);
            }
            catch (SocketException)
            {
                CloseConnection("This error");
                //StartConnection();
                Thread.Sleep(3000);
            }
            catch (Exception)
            {
                CloseConnection("Sudden connection lost");
            }
        }

        protected void ReceiveCallback(IAsyncResult AR)
        {
            Socket currentSocket = (Socket)AR.AsyncState;
            int receivedBytesCount = 0;
            try
            {
                receivedBytesCount = currentSocket.EndReceive(AR);
            }
            catch (SocketException)
            {
                CloseConnection("Couldn't receive the message(SocketException)");
                return;
            }
            catch (ObjectDisposedException)
            {
                CloseConnection("Couldn't receive the message(ObjDisposed)");
                return;
            }
            byte[] tempBuffer = new byte[receivedBytesCount];
            Array.Copy(incomingBuffer, tempBuffer, receivedBytesCount);
            string text = Encoding.ASCII.GetString(tempBuffer);
            IncomingText = text;
            InvokeMessageReceived(currentSocket, EventArgs.Empty);
            ReceiveResponse(currentSocket);
        }

        public static bool IsIPFormatCorrect(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }
            string[] splitIP = ipString.Split('.');
            if (splitIP.Length != 4)
            {
                return false;
            }
            return splitIP.All(splitNumber => byte.TryParse(splitNumber, out byte byteTester));
        }
        public void CloseConnection()
        {
            IsConnected = false;
            mainSocket.Close();
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Always Shutdown before closing
            //current.Shutdown(SocketShutdown.Both);
            //current.Close();
            //clientSockets.Remove(current);
            //Console.WriteLine("Client disconnected");
            //return;
        }

        protected void CloseConnection(string textToDisplay)
        {
            CloseConnection();
            OutputText = textToDisplay;
        }

        public void InvokeMessageReceived(Socket currentSocket, EventArgs e)
        {
            MessageReceived?.Invoke(currentSocket, e);
        }

        public event EventHandler MessageReceived;

    }
}
