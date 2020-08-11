using DESLibrary;
using Int65Extension;
using System;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Client_DES_
{
    class Program : Host
    {
        static TcpClient client = null;
        static NetworkStream stream = null;
        static Int64[] Keys = new Int64[3];

        static void Main(string[] args)
        {

            try
            {
                client = new TcpClient(currentIp, port);
                stream = client.GetStream();

                for (int i = 0; i < 3; i++)
                    Keys[i] = CreateKey(i + 1);

                Console.WriteLine("Сообщения будун шифроваться по протоколу DES!");
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine("Запущен закрытый чат!");
                SendMessage(stream, Keys);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect(client, stream);
            }
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
                        Console.WriteLine("Сервер: {0}", message);
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    Disconnect(client, stream);
                }
            }
        }

        static Int64 CreateKey(int number)
        {
            Int64 g = 0, p = 0, b = 0, Key = 0;
            string message = "";
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        message = ReceiveInformation(stream);
                        message += ' ';
                        g = Convert.ToInt64(LongArithmetic.GiveLong(ref message));
                        p = Convert.ToInt64(LongArithmetic.GiveLong(ref message));
                        Console.WriteLine("Получены параметры g = {0} и p = {1}", g, p);

                        Random random = new Random();
                        b = LongArithmetic.NextInt64(random);
                        BigInteger B = BigInteger.ModPow(g, b, p);
                        Console.WriteLine("Отправка параметра B = " + B);
                        message = "B = " + B;
                        SendInformation(message, stream);
                        break;

                    case 1:
                        message = ReceiveInformation(stream);
                        message += ' ';
                        BigInteger A = BigInteger.Parse(LongArithmetic.GiveLong(ref message));
                        Console.WriteLine("Получен параметр A = {0}", A);
                        Key = Convert.ToInt64(BigInteger.ModPow(A, b, p).ToString());
                        Key = LongArithmetic.GetFormatKey(Key);
                        Console.WriteLine("Получен общий ключ № {0}: Key = {1}", number, Key);
                        break;
                }
            }
            return Key;
        }
    }
}
