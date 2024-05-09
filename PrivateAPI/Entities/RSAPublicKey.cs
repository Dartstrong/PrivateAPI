using PrivateAPI.HelperClasses;
namespace PrivateAPI.Entity
{
    public class RSAPublicKey : Converter
    {
        public string ModulusStr { get { return ModulusStr; } set { Modulus = StrToIntArrayToByteArray(value); } }
        public string ExponentStr { get { return ExponentStr; } set { Exponent = StrToIntArrayToByteArray(value); } }
        public byte[] Modulus { get; private set; }
        public byte[] Exponent { get; private set; }
    }
}
