using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Models
{
    public class Dialogue
    {
        public int Id { get; set; }
        public string Started { get; set; }
        public int StartedDeviceId { get; set; }
        public string  Accepted { get; set; }
        public int AcceptedDeviceId{ get; set; }
        public DateTime LastMessage { get; set; }
    }
}
