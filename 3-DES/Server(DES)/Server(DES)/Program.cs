using DESLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Server_DES_
{
    class ClientObject : Host
    {
        static TcpClient client = null;
        static NetworkStream stream = null;
        static protected Int64[] Keys = new Int64[3];

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            stream = client.GetStream();
        }

        public void Procces()
        {
            for (int i = 0; i < 3; i++)
                Keys[i] = CreateKeys(i + 1);

            Console.WriteLine("Сообщения будун шифроваться по протоколу DES!");
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            Console.WriteLine("Запущен закрытый чат!");
            receiveThread.Start();
            SendMessage(stream, Keys);
        }

        Int64 CreateKeys(int number)
        {
            Console.WriteLine("Создание общего ключа № {0}...", number);
            Random random = new Random();
            Int64 g = LongArithmetic.NextInt64(random);
            Int64 p = LongArithmetic.NextInt64(random);

            Console.WriteLine("Отправка параметров g = " + g + " p = " + p);
            string message = "";
            message = "g = " + g.ToString() + " p = " + p.ToString();
            SendInformation(message, stream);

            Int64 a = LongArithmetic.NextInt64(random);
            BigInteger A = BigInteger.ModPow(g, a, p);
            Console.WriteLine("Отправка параметра A = " + A);
            message = "A = " + A;
            SendInformation(message, stream);

            message = ReceiveInformation(stream);
            message += ' ';
            BigInteger B = BigInteger.Parse(LongArithmetic.GiveLong(ref message));
            Console.WriteLine("Получен параметр B = {0}", B);

            Int64 Key = Convert.ToInt64(BigInteger.ModPow(B, a, p).ToString());
            Key = LongArithmetic.GetFormatKey(Key);
            Console.WriteLine("Получен общий ключ № {0}: Key = {1}", number, Key);
            var buffer = new byte[8];
            buffer = BitConverter.GetBytes(Key);
            return Key;
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        string message = "";
                        byte[] data = new byte[8];
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            string letter = Encoding.Default.GetString(data, 0, bytes);
                            DES translater = new DES(letter, Keys);
                            letter = translater.TripleDecrypt();
                            message += letter;
                        }
                        while (stream.DataAvailable);
                        FilterMessage(ref message);
                        Console.WriteLine("Клиент: {0}", message);
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    Disconnect(client, stream);
                }
            }
        }
    }

    class Server : Host
    {
        static TcpListener server = null;

        static void Main(string[] args)
        {
            try
            {
                // Запускаем сервер
                IPAddress localAddress = IPAddress.Parse(currentIp);
                server = new TcpListener(localAddress, port);
                server.Start();

                Console.WriteLine("Сервер запущен!\nОжидание подключения клиента...");
                TcpClient client = null;
                while ((client = server.AcceptTcpClient()) == null)
                { }

                // Подключаем клиента
                ClientObject clientobj = new ClientObject(client);
                clientobj.Procces();
                Console.WriteLine("Клиент вышел из чата!\n Нажмите ENTER для завершения работы!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect(server);
            }
        }
    }
}
