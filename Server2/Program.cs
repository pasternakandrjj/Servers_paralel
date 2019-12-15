using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool free = true;
            int port = 8006; // для прийшому 
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (free)
            {
                try
                {
                    free = false;
                    // з*єднюємо сокет з локальною точкою
                    listenSocket.Bind(ipPoint);
                    listenSocket.Listen(10);
                    Console.WriteLine("Сервер 2 запущено");
                    while (true)
                    {
                        Socket handler = listenSocket.Accept();
                        // отримуємо повідомлення
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        byte[] data = new byte[256]; // буфер для даних 
                        do
                        {
                            bytes = handler.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (handler.Available > 0);
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                        int a = Int32.Parse(builder.ToString());
                        Console.WriteLine($"Thread gooing to sleep { a * 1000}");

                        Thread.Sleep(a * 1000);
                        Console.WriteLine($"Thread slept { a * 1000}");
                        //закриваємо
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        free = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    free = true;
                }
            }
        }
    }
}
