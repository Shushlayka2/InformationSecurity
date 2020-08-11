using DESLibrary;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Host
{
    static protected int port = 8888;
    static protected string currentIp = "ip-address"; // enter curr ip address

    static protected void SendMessage(NetworkStream stream, Int64[] Keys)
    {
        Console.WriteLine("Введите сообщение: ");
        while (true)
        {
            string message = Console.ReadLine();
            string[] splited = MessageSplit(message);
            string letter = "";
            for (int i = 0; i < splited.Length; i++)
            {
                DES translater = new DES(splited[i], Keys);
                letter += translater.TripleEncrypt();
            }
            byte[] data = Encoding.Default.GetBytes(letter);
            stream.Write(data, 0, data.Length);
        }
    }

    static string[] MessageSplit(string mes)
    {
        string[] result;
        if (mes.Length % 8 != 0)
            result = new string[mes.Length / 8 + 1];
        else
            result = new string[mes.Length / 8];
        int i = 0;
        while (mes != "")
        {
            result[i] = "";
            for (int j = 0; j < 8; j++)
            {
                if (mes != "")
                {
                    result[i] += mes[0];
                    mes = mes.Remove(0, 1);
                }
                else
                    result[i] += ' ';
            }
            i++;
        }
        return result;
    }

    static protected void FilterMessage(ref string message)
    {
        while (message[message.Length - 1] == ' ')
            message = message.Remove(message.Length - 1, 1);
    }

    static protected void SendInformation(string message, NetworkStream stream)
    {
        byte[] data = new byte[64];
        data = Encoding.Unicode.GetBytes(message);
        stream.Write(data, 0, data.Length);
        Thread.Sleep(10);
    }

    static protected string ReceiveInformation(NetworkStream stream)
    {
        string message = "";
        bool found = false;
        while (!found)
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            if (builder.Length != 0)
            {
                message = builder.ToString();
                found = true;
            }
        }
        return message;
    }

    static protected void Disconnect(TcpClient client, NetworkStream stream)
    {
        if (stream != null)
            stream.Close();

        if (client != null)
            client.Close();

        Environment.Exit(0);
    }

    static protected void Disconnect(TcpListener listener)
    {
        if (listener != null)
            listener.Stop();

        Environment.Exit(0);
    }
}
