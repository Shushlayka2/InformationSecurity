using System;
using System.Numerics;

public class MRTest
{
    static public bool Test(BigInteger n)
    {
        int s = 0;
        BigInteger t = n - 1;
        Partition(ref t, ref s);

        int j;
        for (j = 0; j < 10; j++)
        {
            Random rand = new Random();
            int a = 0;
            try
            { a = rand.Next(2, (int)(n - 1)); }
            catch
            { a = rand.Next(2, int.MaxValue); }


            BigInteger x = BigInteger.ModPow(a, t, n);
            if ((x != 1) && (x != (n - 1)))
            {
                int i;
                for (i = 0; i < s; i++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == (n - 1))
                        break;
                }
                if (i == s)
                    return false;
            }
        }
        return true;
    }
    static void Partition(ref BigInteger t, ref int s)
    {
        while (t % 2 != 1)
        {
            s++;
            t = t / 2;
        }
    }
}
