using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Entity
{
    public class RSAPublicKey
    {
        public string ModulusStr { get { return ModulusStr; } set { Modulus = StrToByteArray(value); } }
        public string ExponentStr { get { return ExponentStr; } set { Exponent = StrToByteArray(value); } }
        public byte[] Modulus { get; private set; }
        public byte[] Exponent { get; private set; }
        private static byte[] StrToByteArray(string str)
        {
            string[] arrayStr = str.Split('-');
            int[] arrayInt = new int[arrayStr.Length];
            for (int i = 0; i < arrayStr.Length; i++)
            {
                arrayInt[i] = Convert.ToInt32(arrayStr[i]);
            }
            byte[] arrayByte = new byte[arrayInt.Length];
            for (int i = 0; i < arrayByte.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(arrayInt[i]);
            }
            return arrayByte;
        }
    }
}
