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
            TCPlient client = new TCPlient(portNumber: 100);
            string text = "";
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            client.Disconnected += Client_Disconnected;
            while (true)
            {
                timer.Elapsed += Timer_Elapsed;
                Console.WriteLine("Type 'connect' to get connected");
                text = Console.ReadLine();
                //timer.Start();
                if (text.ToLower() != "connect") continue;
                while (!client.IsConnected)
                {
                    client.StartConnection();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Connecting to the server");
                    Thread.Sleep(2000);
                }
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine("Connected to the server");

                timer.Start();
                while (text != "exit")
                {
                    if (!client.IsConnected) break;
                    text = Console.ReadLine();
                    if (text == "exit") break;
                    else
                    {
                        client.SendString(text);
                        Thread.Sleep(100);
                        Console.WriteLine(client.IncomingText);
                    }
                }
                timer.Stop();
                void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
                {
                    //client.SendString("");
                }
            }
                void Client_Disconnected(object sender, EventArgs e)
                {
                    Console.WriteLine("Disconnected. Type 'exit'");
                    timer.Stop();
                }
        }


    }
}
