using System;
namespace PrivateAPI.Models
{
    public class NewDialogue
    {
        public int Id { get; set; }
        public int Sender { get; set; }
        public int Receiver { get; set; }
        public string PublicKeyModulus { get; set; }
        public string PublicKeyExponent { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
