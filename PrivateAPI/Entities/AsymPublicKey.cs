using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Entity
{
    public class AsymPublicKey
    {
        public string StrKey
        {
            get
            {
                return StrKey;
            }
            set
            {
                ByteArrayKey = StrToByteArray(value);
            }
        }
        public byte[] ByteArrayKey { get; private set; }
        private static byte[] StrToByteArray(string strKey)
        {
            string[] arrayStr = strKey.Split('-');
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
