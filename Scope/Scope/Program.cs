using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Scope
{
    static public class Whois
    {
        const int Whois_Server_Default_PortNumber = 43;
        const string DotCom_Wois_Server = "whois.verisign-grs.com";
        static public string Record_Type;

        static public string Lookup(string domainName)
        {
            using (TcpClient whoisClient = new TcpClient())
            {
                whoisClient.Connect(DotCom_Wois_Server, Whois_Server_Default_PortNumber);

                string domainQuery = Record_Type + " " + domainName + "\r\n";
                byte[] domainQueryBytes = Encoding.ASCII.GetBytes(domainQuery.ToCharArray());

                Stream whoisStream = whoisClient.GetStream();
                whoisStream.Write(domainQueryBytes, 0, domainQueryBytes.Length);

                StreamReader whoisStreamReader = new StreamReader(whoisStream, Encoding.ASCII);

                string streamOutputContent = "";
                List<string> whoisData = new List<string>();
                while ((streamOutputContent = whoisStreamReader.ReadLine()) != null)
                    whoisData.Add(streamOutputContent);

                whoisClient.Close();

                return String.Join(Environment.NewLine, whoisData);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a domain name for scope creating.");
            string domainName = Console.ReadLine();
            try
            {
                Whois.Record_Type = "domain";
                string whoisText = Whois.Lookup(domainName);
                Parser(whoisText, domainName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Parser(string str, string domain)
        {
            string result = "\r\n";
            result += "Domain Name: " + str.CopeAfter("Domain Name") + "\r\n";
            result += "Registrar: " + str.CopeAfter("Registrar") + "\r\n";
            result += "Updated Date: " + str.CopeAfter("Updated Date") + "\r\n";
            result += "Creation Date: " + str.CopeAfter("Creation Date") + "\r\n";
            result += "Expiration Date: " + str.CopeAfter("Expiration Date") + "\r\n\r\n";
            result += "Ports:\r\n";
            result += Nmap(domain) + "\r\n";
            result += "Servers:";
            Console.WriteLine(result);

            string server = "";
            while ((server = StringExtension.SearchAndDelete(ref str, "Name Server: ")) != "")
            {
                result = "Name Server: " + server + "\r\n";
                Whois.Record_Type = "nameserver";
                string server_information = Whois.Lookup(server);

                string ip = "";
                while (server_information.IndexOf("IP Address") != -1)
                {
                    ip = StringExtension.SearchAndDelete(ref server_information, "IP Address: ");
                    if ((ip != "") && (ip.IndexOf(':') == -1))
                    {
                        result += "IP Address: " + ip + "\r\n";
                        result += "Port: " + Nmap(ip);
                    }
                }
                Console.WriteLine(result);
            }
        }

        static string Nmap(string obj)
        {
            Process cmd = new Process();
            cmd.StartInfo = new ProcessStartInfo(@"cmd.exe");
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("nmap " + obj);

            string NmapResult = "";
            string s = cmd.StandardOutput.ReadLine();
            while (s.IndexOf("done") == -1)
            {
                NmapResult += s + "\r\n";
                s = cmd.StandardOutput.ReadLine();
            }
            NmapResult += s;

            cmd.Close();

            return PortsSearch(ref NmapResult);
        }

        static string PortsSearch(ref string str)
        {
            string result = "";
            while (str.IndexOf("PORT") != -1)
                str = str.Remove(0, str.IndexOf("\r\n") + 2);
            while (str.IndexOf("Nmap done") != 2)
            {
                result = str.Substring(0, str.IndexOf("\r\n") + 2);
                str = str.Remove(0, str.IndexOf("\r\n") + 2);
            }
            return result;
        }
    }

    public static class StringExtension
    {
        public static string CopeAfter(this string str, string substring)
        {
            string result = "";
            substring += ": ";
            if (str.IndexOf(substring) != -1)
            {
                str = str.Remove(0, str.IndexOf(substring) + substring.Length);
                result = str.Substring(0, str.IndexOf("\r\n"));
            }
            return result;
        }

        public static string SearchAndDelete(ref string str, string substring)
        {
            string result = "";
            if (str.IndexOf(substring) != -1)
            {
                str = str.Remove(0, str.IndexOf(substring) + substring.Length);
                result = str.Substring(0, str.IndexOf("\r\n"));
                str = str.Remove(0, str.IndexOf("\r\n") + 2);
            }
            return result;
        }
    }
}
