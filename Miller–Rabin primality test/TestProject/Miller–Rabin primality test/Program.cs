using System;
using System.Numerics;

namespace Miller_Rabin_primality_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 32766;
            Console.WriteLine("Тест Миллера - Рабина с 10 раундами!");
            BigInteger A, B = new BigInteger();
            string input = Console.ReadLine();
            if (input.IndexOf(' ') != -1)
            {
                int index = input.IndexOf(' ');
                A = BigInteger.Parse(input.Substring(0, index));
                input = input.Remove(0, index + 1);
                B = BigInteger.Parse(input);
            }
            else
            {
                A = BigInteger.Parse(input);
                B = A;
            }
            CheckInterval(A, B);
            Console.WriteLine("Нажмите ENTER для завершения работы...");
            Console.ReadKey();
        }

        static public void CheckInterval(BigInteger A, BigInteger B)
        {
            while (A <= B)
            {
                if ((A == 1) || (A == 2))
                    Console.WriteLine("Число {0} - простое!", A);
                else if (A % 2 != 0)
                {
                    bool answer = MRTest.Test(A);
                    if (answer)
                        Console.WriteLine("Число {0} - вероятно простое!", A);
                    else
                        Console.WriteLine("Число {0} - составное!", A);
                }
                else
                    Console.WriteLine("Число {0} - составное!", A);
                A++;
            }
        }
    }
}
