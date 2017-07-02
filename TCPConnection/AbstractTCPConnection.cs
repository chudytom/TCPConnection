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
        protected Socket mainSocket;
        public bool IsConnected { get; protected set; }
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
                //CloseConnection();
                return;
            }
            finally
            {
                IsConnected = mainSocket.Connected;
            }
            //catch (ArgumentException)
            //{
            //    OutputText = "Cannot connect to the server(end, ArgumentException)";
            //    return;
            //}
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
                Point[] points = { new Point(100, 200),
                         new Point(150, 250), new Point(250, 375),
                         new Point(275, 395), new Point(295, 450) };

                // Define the Predicate<T> delegate.
                Predicate<Point> predicate = FindPoints;
                bool FindPoints(Point obj)
                {
                    return obj.X * obj.Y > 100000;
                }
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
            //catch (Exception)
            //{
            //    CloseConnection("Sudden connection lost");
            //}
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
                //mainSocket.Disconnect(true);
                //IsConnected = false;
                if (IsConnected == true)
                    CloseConnection("Couldn't receive the message(SocketException)");
                return;
            }
            catch (ObjectDisposedException)
            {
                //mainSocket.Disconnect(true);
                //IsConnected = false;
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
        public void CloseConnection()
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
