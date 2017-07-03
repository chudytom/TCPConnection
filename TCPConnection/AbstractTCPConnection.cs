using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

namespace TCPConnection
{
    public abstract class AbstractTCPConnection
    {
        abstract public void StartConnection();

        public bool IsConnected { get; protected set; }
        public string IP { get; set; } = IPAddress.Loopback.ToString();
        public int PortNumber { get; set; }
        public string OutputText { get; set; }
        public string IncomingText { get; set; }

        protected Socket mainSocket;
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
            finally
            {
                IsConnected = mainSocket.Connected;
            }
            ReceiveResponse(mainSocket);
            OutputText = "Connected";
        }

        public void SendString(string stringToSend)
        {
            byte[] outgoingBuffer = Encoding.ASCII.GetBytes(stringToSend);
            try
            {
                mainSocket?.Send(outgoingBuffer, 0, outgoingBuffer.Length, SocketFlags.None);
            }
            catch (SocketException)
            {
                OutputText = "Unable to send string (SocketException)";
                return;
            }
            catch (ObjectDisposedException)
            {
                OutputText = "Unable to send string (SocketException)";
                return;
            }
        }
        /// <summary>
        /// BeginReceive method with exception handling
        /// </summary>
        /// <param name="socket"></param>
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
                Thread.Sleep(3000);
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
                if (IsConnected == true)
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
            OnMessageReceived(currentSocket, EventArgs.Empty);
            ReceiveResponse(currentSocket);
        }

        public static bool IsIPFormatCorrect(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }
            string[] ipAfterSplit = ipString.Split('.');
            if (ipAfterSplit.Length != 4)
            {
                return false;
            }
            return ipAfterSplit.All(splitNumber => byte.TryParse(splitNumber, out byte byteTester));
        }
        protected void CloseConnection()
        {
            IsConnected = false;
            mainSocket.Shutdown(SocketShutdown.Both);
            mainSocket.Disconnect(false);
            OnDisconnected(this, EventArgs.Empty);
        }

        protected void CloseConnection(string textToDisplay)
        {
            CloseConnection();
            OutputText = textToDisplay;
        }

        public event EventHandler MessageReceived;
        protected void OnMessageReceived(Socket currentSocket, EventArgs e)
        {
            MessageReceived?.Invoke(currentSocket, e);
        }

        public event EventHandler Disconnected;
        protected void OnDisconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(sender, e);
        }
    }
}
