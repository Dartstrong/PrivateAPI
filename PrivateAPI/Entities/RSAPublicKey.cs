using PrivateAPI.HelperClasses;
namespace PrivateAPI.Entities
{
    public class RSAPublicKey : Converter
    {
        public string ModulusStr { get; set;  }
        public string ExponentStr { get; set; }

    }
}
