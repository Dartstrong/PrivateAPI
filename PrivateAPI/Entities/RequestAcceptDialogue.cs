using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Entities
{
    public class RequestAcceptDialogue
    {
        public string Accepted { get; set; }
        public string AcceptedPassword { get; set; }
        public string AcceptedDeviceId { get; set; }
        public string PublicKeyModulus { get; set; }
        public string PublicKeyExponent { get; set; }
    }
}
