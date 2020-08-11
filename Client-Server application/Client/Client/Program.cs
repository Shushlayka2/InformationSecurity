using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static int port = 8005;

        static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("ip-address"), port); // enter curr ip address
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(ipPoint);
                while (true)
                {
                    Console.WriteLine("Введите сообщение:");

                    String message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);

                    socket.Send(data);

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    data = new byte[256];

                    do
                    {
                        bytes = socket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);

                    Console.WriteLine("Ответ сервера: " + builder.ToString());
                }
                //  socket.Shutdown(SocketShutdown.Both);
                //  socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
