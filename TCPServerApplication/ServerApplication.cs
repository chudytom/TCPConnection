using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPConnection;

namespace TCPServerApplication
{
    class ServerApplication
    {
        static TCPServer server = new TCPServer(100);
        static void Main(string[] args)
        {
            server.StartConnection();
            while (!server.IsConnected)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Waiting for a client to connect");
                Thread.Sleep(500);
            }
            server.MessageReceived += Server_MessageReceived;
            Console.ReadLine();
        }

        private static void Server_MessageReceived(object sender, EventArgs e)
        {
            Socket currentSocket = (Socket)sender;
            string textToSend = "";
            if (server.IncomingText == "get time")
            {
                textToSend = DateTime.Now.ToLongTimeString();
                Console.WriteLine("Time sent to client");
            }
            else
            {
                textToSend = "Send another request";
                Console.WriteLine("Invalid request");
            }
            currentSocket.Send(Encoding.ASCII.GetBytes(textToSend));
        }
    }
}
