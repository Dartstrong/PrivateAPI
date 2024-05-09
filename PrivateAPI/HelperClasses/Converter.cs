using System;
using System.Text;
namespace PrivateAPI.HelperClasses
{
    public class Converter
    {
        protected static byte[] StrToIntArrayToByteArray(string str)
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
        protected static string ByteArrayToIntArrayToStr(byte[] data)
        {
            int[] arrayInt = new int[data.Length];
            for (int i = 0; i < arrayInt.Length; i++)
                arrayInt[i] = Convert.ToInt32(data[i]);
            StringBuilder sb = new("");
            for (int i = 0; i < arrayInt.Length; i++)
            {
                sb.Append(arrayInt[i]);
                sb.Append('-');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
