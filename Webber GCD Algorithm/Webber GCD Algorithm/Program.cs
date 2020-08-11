using System;
using System.Collections.Generic;
using System.Numerics;

namespace Webber_GCD_Algorithm
{
    class Pair
    {
        public BigInteger n { get; set; }
        public BigInteger d { get; set; }

        public Pair(BigInteger n, BigInteger d)
        {
            this.n = n;
            this.d = d;
        }

        public static void Swap(ref Pair a, ref Pair b)
        {
            Pair tempt = a;
            a = b;
            b = tempt;
        }

        public static Pair operator -(Pair a, Pair b)
        {
            return new Pair(a.n - b.n, a.d - b.d);
        }

        public static Pair operator *(BigInteger b, Pair a)
        {
            return new Pair(a.n * b, a.d * b);
        }
    }

    class Program
    {
        const int W = 64;

        static void Main(string[] args)
        {
            Console.WriteLine("Алгоритм нахождения НОД(u, v)");
            Console.WriteLine("Введите значения u, v и параметров s и t");
            string[] tempt = Console.ReadLine().Split(' ');
            BigInteger u0 = BigInteger.Parse(tempt[0]);
            BigInteger v0 = BigInteger.Parse(tempt[1]);
            BigInteger s = BigInteger.Parse(tempt[2]);
            int t = int.Parse(tempt[3]);

            BigInteger g = 1;
            try
            {
                CheckEntireParams(ref u0, ref v0, ref g);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            BigInteger k = BigInteger.Pow(10, 2 * t);
            BigInteger u = u0;
            BigInteger v = v0;
            while (v != 0)
            {
                if (u.ToString().Length - v.ToString().Length > s)
                    u = Dmod(u, v);
                else
                {
                    Pair p = ReducedRatMod(u, v, k, t);
                    u = BigInteger.Abs(p.n * v - p.d * u) / k;
                }
                u = RemoveFactors(u);
                Swap(ref u, ref v);
            }
            BigInteger x = BigInteger.GreatestCommonDivisor(Dmod(v0, u), u);
            Console.WriteLine(g * BigInteger.GreatestCommonDivisor(Dmod(u0, x), x));
            Console.ReadKey();
        }

        static void CheckEntireParams(ref BigInteger u, ref BigInteger v, ref BigInteger g)
        {
            if ((u <= 0) || (v <= 0))
                throw new Exception("Параметры должны быть положительными!");

            int[] d = new int[3] { 10, 5, 2 };
            for (int i = 0; i < 3; i++)
            {
                while ((u % d[i] == 0) && (v % d[i] == 0))
                {
                    u = u / d[i];
                    v = v / d[i];
                    g *= d[i];
                }
            }

            u = RemoveFactors(u);
            v = RemoveFactors(v);

            int und = u.ToString().Length;
            int vnd = v.ToString().Length;

            if (und < vnd)
            {
                u = u + v; und = und + vnd;
                v = u - v; vnd = und - vnd;
                u = u - v; und = und - vnd;
            }

        }

        static BigInteger RemoveFactors(BigInteger a)
        {
            int[] d = new int[3] { 10, 5, 2 };
            if (a != 0)
                for (int i = 0; i < 3; i++)
                    while (a % d[i] == 0)
                        a = a / d[i];
            return a;
        }

        static BigInteger Dmod(BigInteger u0, BigInteger v0)
        {
            BigInteger u = u0;
            int und = u.ToString().Length;
            int vnd = v0.ToString().Length;
            BigInteger modul = BigInteger.Pow(10, W);
            while (und >= vnd + W)
            {
                if (BigInteger.Remainder(u, modul) != 0)
                    u = BigInteger.Abs(u - BigInteger.Remainder(u * ExtendedEuclidsAlgorithm(modul, BigInteger.Remainder(v0, modul)), modul) * v0);
                u = u / modul;
            }

            int d = und - vnd;
            modul = BigInteger.Pow(10, d + 1);
            if (BigInteger.Remainder(u, modul) != 0)
                u = BigInteger.Abs(u - BigInteger.Remainder(u * ExtendedEuclidsAlgorithm(modul, BigInteger.Remainder(v0, modul)), modul) * v0);
            return (u / modul);
        }

        static BigInteger ExtendedEuclidsAlgorithm(BigInteger A, BigInteger B)
        {
            List<BigInteger> ListOfDiv = GetListOfDiv(A, B);
            BigInteger X = new BigInteger();
            BigInteger Y = new BigInteger();
            X = 0; Y = 1;
            for (int i = ListOfDiv.Count - 1; i >= 0; i--)
            {
                BigInteger temporary = X;
                X = Y;
                Y = temporary - (BigInteger.Multiply(Y, ListOfDiv[i]));
            }
            if (Y.Sign == -1)
                Y = A + Y;
            return Y;
        }

        static List<BigInteger> GetListOfDiv(BigInteger A, BigInteger B)
        {
            List<BigInteger> Result = new List<BigInteger>();
            BigInteger Remainder = new BigInteger();
            Remainder = BigInteger.Remainder(A, B);
            BigInteger div;
            while (Remainder != 0)
            {
                div = BigInteger.Divide(A, B);
                Result.Add(div);
                A = B;
                B = Remainder;
                Remainder = BigInteger.Remainder(A, B);
            }
            return Result;
        }

        static Pair ReducedRatMod(BigInteger x, BigInteger y, BigInteger m, int t)
        {
            BigInteger c = BigInteger.Remainder(x * ExtendedEuclidsAlgorithm(m, BigInteger.Remainder(y, m)), m);
            Pair f1 = new Pair(m, 0);
            Pair f2 = new Pair(c, 1);
            while (f2.n >= BigInteger.Pow(10, t))
            {
                f1 = f1 - ((f1.n / f2.n) * f2);
                Pair.Swap(ref f1, ref f2);
            }
            return f2;
        }

        static void Swap(ref BigInteger a, ref BigInteger b)
        {
            BigInteger tempt = a;
            a = b; b = tempt;
        }
    }
}
