using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Models
{
    public class NewDialogue
    {
        public int Id { get; set; }
        public int Sender { get; set; }
        public string Receiver { get; set; }
        public string PublicKeyModulus { get; set; }
        public string PublicKeyExponent { get; set; }
    }
}
