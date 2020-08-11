using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

static public class LongArithmetic
{
    static public Int64 NextInt64(Random random)
    {
        var buffer = new byte[8];
        random.NextBytes(buffer);
        Int64 ret = BitConverter.ToInt64(buffer, 0);
        if (ret < 0)
            ret *= -1;
        return ret;
    }

    /*   static public Int64 NextInt64(Random random)
       {
           bool found = false;
           Int64 ret = 0;
           while (!found)
           {
               var buffer = new byte[8];
               random.NextBytes(buffer);
               ret = BitConverter.ToInt64(buffer, 0);
               if ((ret % 2 != 0) && (MRTest.Test((BigInteger)ret)))
                   found = true;
           }
           return ret;
       }*/

    static public string GiveLong(ref string str)
    {
        str = str.Remove(0, str.IndexOf('=') + 2);
        string ret;
        ret = str.Substring(0, str.IndexOf(' '));
        str = str.Remove(0, str.IndexOf(' ') + 1);
        return ret;
    }

    static public Int64 GetFormatKey(Int64 key)
    {
        var buffer = new byte[8];
        buffer = BitConverter.GetBytes(key);
        buffer[7] = 0;
        key = BitConverter.ToInt64(buffer, 0);
        return key;
    }
}

static public class Bits
{
    static public BitArray ToBinary(Int64 num, int size)
    {
        BitArray result = new BitArray(size, false);
        Int64 remainder = 0;
        int i = size - 1;
        while ((num != 0) && (num != 1))
        {
            remainder = num % 2;
            if (remainder == 1)
                result[i] = true;
            num = num / 2;
            i--;
        }
        result[i] = true;
        return result;
    }

    static public BitArray ToBinary(string text, int size)
    {
        byte[] data = new byte[size];
        data = Encoding.Default.GetBytes(text);
        return ToBits(data);
    }

    static BitArray ToBits(byte[] data)
    {
        BitArray result = new BitArray(data.Length * 8);
        int index = 1;
        foreach (int one in data)
        {
            int i = index * 8 - 1, remainder = 0, num = one;
            while ((num != 0) && (num != 1))
            {
                remainder = num % 2;
                if (remainder == 1)
                    result[i] = true;
                num = num / 2;
                i--;
            }
            index++;
            result[i] = true;
        }
        return result;
    }

    static public int ToNumeral(BitArray num)
    {
        int result = 0;
        int index = num.Count - 1;
        foreach (bool b in num)
        {
            if (b)
                result += Convert.ToInt32(Math.Pow(2, index));
            index--;
        }
        return result;
    }

    static public string ToStr(BitArray bits)
    {
        int numBytes = bits.Count / 8;
        if (bits.Count % 8 != 0) numBytes++;

        byte[] bytes = new byte[numBytes];
        int byteIndex = 0, bitIndex = 0;

        for (int i = 0; i < bits.Count; i++)
        {
            if (bits[i])
                bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

            bitIndex++;
            if (bitIndex == 8)
            {
                bitIndex = 0;
                byteIndex++;
            }
        }
        string result = Encoding.Default.GetString(bytes);
        return result;
    }

    static public void LeftShift(ref BitArray Arr)
    {
        bool first = Arr[0];
        for (int i = 0; i < 27; i++)
            Arr[i] = Arr[i + 1];
        Arr[27] = first;
    }

    static public void RightShift(ref BitArray Arr)
    {
        bool last = Arr[27];
        for (int i = 27; i > 0; i--)
            Arr[i] = Arr[i - 1];
        Arr[0] = last;
    }

    static public BitArray Association(BitArray L, BitArray R, int size)
    {
        BitArray result = new BitArray(size);
        for (int i = 0; i < (size / 2); i++)
            result[i] = L[i];
        for (int i = (size / 2); i < size; i++)
            result[i] = R[i - (size / 2)];
        return result;
    }

    static public BitArray Association(List<BitArray> list)
    {
        BitArray result = new BitArray(32);

        int i = 0;
        foreach (BitArray b in list)
        {
            for (int j = 0; j < 4; j++)
            {
                result[i] = b[j];
                i++;
            }
        }
        return result;
    }

    static public List<BitArray> Splitting(BitArray WholeBlock)
    {
        List<BitArray> ResultList = new List<BitArray>();
        for (int i = 0; i < 8; i++)
        {
            BitArray b = new BitArray(6);
            for (int j = 0; j < 6; j++)
                b[j] = WholeBlock[(i * 6) + j];
            ResultList.Add(b);
        }
        return ResultList;
    }
}