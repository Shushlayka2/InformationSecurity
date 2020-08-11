using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sedjelmaci_GCD_Algorithm
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

    class Matrix
    {
        public BigInteger n1 { set; get; }
        public BigInteger n2 { set; get; }
        public BigInteger d1 { set; get; }
        public BigInteger d2 { set; get; }

        public Matrix(BigInteger n1, BigInteger d1, BigInteger n2, BigInteger d2)
        {
            this.n1 = n1; this.d1 = d1;
            this.n2 = n2; this.d2 = d2;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите значения u и v для нахождения НОД(u,v)!");
            string[] tempt = Console.ReadLine().Split(' ');
            BigInteger u = BigInteger.Parse(tempt[0]);
            BigInteger v = BigInteger.Parse(tempt[1]);
            BigInteger k = BigInteger.Pow(2, 64);
            double n = BigInteger.Log(u) / Math.Log(2);
            if (Math.Pow(n, 0.4) > (double)k)
            {
                double m = Math.Truncate(0.4 * (Math.Log(n) / Math.Log(2))) + 1;
                m = m + m % 2; k = BigInteger.Pow(2, (int)m);
            }
            while (u * v != 0)
            {
                if (u < v)
                    Swap(ref u, ref v);
                if ((double)(u / v) < Math.Sqrt((double)k))
                {
                    Matrix M = M_JWA(u, v, k);
                    BigInteger t = u;
                    u = BigInteger.Abs(M.d1 * u - M.n1 * v) / k;
                    v = BigInteger.Abs(M.d2 * t - M.n2 * v) / k;
                }
                else
                {
                    BigInteger t = u;
                    u = v; v = t % v;
                }
                u = Makeodd(u); v = Makeodd(v);
            }
            Console.WriteLine(u + v);
            Console.ReadKey();
        }

        static void Swap(ref BigInteger A, ref BigInteger B)
        {
            BigInteger tempt = A;
            A = B; B = tempt;
        }

        static Matrix M_JWA(BigInteger x, BigInteger y, BigInteger k)
        {
            BigInteger r = BigInteger.Remainder(x * ExtendedEuclidsAlgorithm(k, y), k);
            Pair f1 = new Pair(k, 0);
            Pair f2 = new Pair(r, 1);
            while ((double)f2.n >= Math.Sqrt((double)k))
            {
                f1 = f1 - ((f1.n / f2.n) * f2);
                Pair.Swap(ref f1, ref f2);
            }
            return new Matrix(f1.n, f1.d, f2.n, f2.d);
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

        static BigInteger Makeodd(BigInteger A)
        {
            if (A != 0)
                while (A % 2 == 0)
                    A = A / 2;
            return A;
        }
    }
}
