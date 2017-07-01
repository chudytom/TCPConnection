using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("First client connected");
            server.MessageReceived += Server_MessageReceived;
            Console.ReadLine();
        }

        private static void Server_MessageReceived(object sender, EventArgs e)
        {
            if (server.IncomingText == "get time")
            {
                server.SendString(DateTime.Now.TimeOfDay.ToString());
                Console.WriteLine("Time sent to client");
            }
            else
            {
                server.SendString("Send another request");
                Console.WriteLine("Invalid request");
            }
        }
    }
}
