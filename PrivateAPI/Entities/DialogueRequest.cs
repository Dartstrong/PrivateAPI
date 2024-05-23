using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Entities
{
    public class DialogueRequest
    {
        public string IdStr { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}
