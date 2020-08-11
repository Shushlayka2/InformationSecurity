using System;
using System.Collections;
using System.Collections.Generic;

namespace DESLibrary
{
    static internal class Message
    {
        static internal BitArray content;
        static internal BitArray LeftSide = new BitArray(32);
        static internal BitArray RightSide = new BitArray(32);

        static internal void GetLeftAndRight()
        {
            for (int i = 0; i < 64; i++)
                if (i < 32)
                    LeftSide[i] = content[i];
                else
                    RightSide[i - 32] = content[i];
        }

        static internal void Encryption(Key key)
        {
            BitArray Lcopy = LeftSide;
            LeftSide = RightSide;
            RightSide = Lcopy.Xor(MainFunction(RightSide, key));
        }

        static internal void Decryption(Key key)
        {
            BitArray Rcopy = RightSide;
            RightSide = LeftSide;
            LeftSide = Rcopy.Xor(MainFunction(LeftSide, key));
        }

        static BitArray MainFunction(BitArray vector, Key key)
        {
            vector = Tables.Transformation(vector, Tables.ListE, 48); //функция расширения E

            //сложение по модулю 2 с ключом
            BitArray B = vector.Xor(key.Value);
            List<BitArray> ListOfB = Bits.Splitting(B);

            //преобразование S
            int i = 1;
            List<BitArray> ListOfChangedB = new List<BitArray>();
            foreach (BitArray b in ListOfB)
            {
                BitArray changedb = Tables.TransformationByTable(b, i);
                ListOfChangedB.Add(changedb);
                i++;
            }
            vector = Bits.Association(ListOfChangedB);

            // Перестановка P
            vector = Tables.Transformation(vector, Tables.ListP, 32);

            return vector;
        }
    }

    internal class Key
    {
        public BitArray Value { get; }
        static BitArray C;
        static BitArray D;

        public Key(BitArray value)
        {
            Value = value;
        }

        internal void GenCAndD()
        {
            BitArray ConvertedKey = new BitArray(64);
            int quantity = 0, shift = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((i != 0) && (i % 8 == 7))
                {
                    if (quantity % 2 == 0)
                        ConvertedKey[i] = true;
                    else
                        ConvertedKey[i] = false;
                    quantity = 0;
                    shift++;
                }
                else
                {
                    ConvertedKey[i] = Value[i - shift];
                    if (ConvertedKey[i] == true)
                        quantity++;
                }
            }
            C = Tables.Transformation(ConvertedKey, Tables.ListC, 28);
            D = Tables.Transformation(ConvertedKey, Tables.ListD, 28);
        }

        static internal BitArray GetKeyForEncrypt(int index)
        {
            Bits.LeftShift(ref C);
            Bits.LeftShift(ref D);
            if ((index != 0) && (index != 1) && (index != 8) && (index != 15))
            {
                Bits.LeftShift(ref C);
                Bits.LeftShift(ref D);
            }
            BitArray common = Bits.Association(C, D, 56);
            BitArray key = Tables.Transformation(common, Tables.ListKey, 48);
            return key;
        }

        static internal BitArray GetKeyForDecrypt(int index)
        {
            if (index != 0)
            {
                Bits.RightShift(ref C);
                Bits.RightShift(ref D);
                if ((index != 1) && (index != 8) && (index != 15))
                {
                    Bits.RightShift(ref C);
                    Bits.RightShift(ref D);
                }
            }
            BitArray common = Bits.Association(C, D, 56);
            BitArray key = Tables.Transformation(common, Tables.ListKey, 48);
            return key;
        }
    }

    public class DES
    {
        List<Key> KeyList = new List<Key>();

        public DES(string message, Int64[] keys)
        {
            Message.content = Bits.ToBinary(message, 8);

            for (int i = 0; i < 3; i++)
            {
                BitArray temporary = new BitArray(56);
                temporary = Bits.ToBinary(keys[i], 56);
                Key key = new Key(temporary);
                KeyList.Add(key);
            }
        }

        public string TripleEncrypt()
        {
            for (int i = 0; i < 3; i++)
                Encrypt(i);
            return Bits.ToStr(Message.content);
        }

        public string TripleDecrypt()
        {
            for (int i = 2; i > -1; i--)
                Decrypt(i);
            return Bits.ToStr(Message.content);
        }

        public string Encrypt(int index)
        {
            Message.content = Tables.Transformation(Message.content, Tables.ListIP, 64); // Начальная перестановка

            Message.GetLeftAndRight(); // Разбиение на 2 части

            KeyList[index].GenCAndD();

            // Циклы шифрования
            for (int i = 0; i < 16; i++)
            {
                Key intermediateKey = new Key(Key.GetKeyForEncrypt(i));
                Message.Encryption(intermediateKey);
            }
            Message.content = Bits.Association(Message.LeftSide, Message.RightSide, 64);

            Message.content = Tables.Transformation(Message.content, Tables.ListPI, 64); // Конечная перестановка

            return Bits.ToStr(Message.content);
        }

        public string Decrypt(int index)
        {
            Message.content = Tables.Transformation(Message.content, Tables.ListIP, 64); // Начальная перестановка

            Message.GetLeftAndRight(); // Разбиение на 2 части

            KeyList[index].GenCAndD();

            // Циклы дешифрования
            for (int i = 0; i < 16; i++)
            {
                Key intermediateKey = new Key(Key.GetKeyForDecrypt(i));
                Message.Decryption(intermediateKey);
            }
            Message.content = Bits.Association(Message.LeftSide, Message.RightSide, 64);

            Message.content = Tables.Transformation(Message.content, Tables.ListPI, 64); // Конечная перестановка

            return Bits.ToStr(Message.content);
        }
    }
}
