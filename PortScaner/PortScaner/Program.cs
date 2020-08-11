using System;
using System.Net;
using System.Net.Sockets;

namespace PortScaner
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 1; i++)
            {
                string ip = "127.0.0." + i.ToString();
                for (int port = 1; port <= 1000; port++)
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                        Console.Write(ip + ':' + port.ToString() + ' ');
                        socket.Connect(ipPoint, TimeSpan.FromMilliseconds(100));
                        Console.WriteLine("Open");
                    }
                    catch
                    {
                        Console.WriteLine("Closed");
                    }
                }
            }
        }
    }

    public static class SocketExtensions
    {
        public static void Connect(this Socket socket, EndPoint endpoint, TimeSpan timeout)
        {
            var result = socket.BeginConnect(endpoint, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (success)
            {
                socket.EndConnect(result);
                socket.Close();
            }
            else
            {
                socket.Close();
                throw new Exception();
            }
        }
    }
}
