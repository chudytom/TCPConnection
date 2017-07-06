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
            //TCPlient client = new TCPlient(portNumber: 100);
            TCPlient client = new TCPlient(portNumber: 10001, ipAddress: "192.168.10.10");
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            client.Disconnected += Client_Disconnected;
            client.MessageReceived += Client_MessageReceived;
            Console.WriteLine($"Start time {DateTime.Now.ToLongTimeString()}");
            while (true)
            {
                Console.WriteLine("Type 'connect' to get connected");
                string text = Console.ReadLine();
                if (text.ToLower() != "connect") continue;
                while (!client.IsConnected)
                {
                    client.StartConnection();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Connecting to the reader");
                    Thread.Sleep(2000);
                }
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine("Connected to the reader");

                //timer.Start();
                while (text != "exit")
                {
                    if (!client.IsConnected) break;
                    text = Console.ReadLine();
                    if (text == "exit") break;
                    else
                    {
                        client.SendString(text);
                    }
                }
                //timer.Stop();
            }
            void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                client.UpateConnection();
                if (!client.IsConnected)
                    Console.WriteLine("It finally got disconnected");
                client.SendString("w");
                Console.WriteLine("Message has been sent");
            }
            void Client_Disconnected(object sender, EventArgs e)
            {
                Console.WriteLine($"Disconnected. Type 'exit'. Current time {DateTime.Now.ToLongTimeString()}");
                timer.Stop();
            }
            void Client_MessageReceived(object sender, EventArgs e)
            {
                Console.WriteLine(client.IncomingText);
            }
        }

    }
}
