using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TCPConnection;

namespace TCPClientApplication
{
    class ClientApplication
    {
        static void Main(string[] args)
        {
            TCPlient client = new TCPlient(100);
            //client.Start();
            client.StartConnection();
            while (!client.IsConnected)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Connecting to the server");
                Thread.Sleep(500);
            }
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            Console.WriteLine("Connected to the server");
            string text = "";
            while (text!="exit")
            {
                text = Console.ReadLine();
                if (text == "exit") return;
                else
                {
                    client.SendString(text);
                    Thread.Sleep(100);
                    Console.WriteLine(client.IncomingText);
                }
            }
        }
    }
}
