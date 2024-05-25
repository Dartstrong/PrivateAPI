using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Models
{
    public class Dialogue
    {
        public int Id { get; set; }
        public int Started { get; set; }
        public int StartedDeviceId { get; set; } 
        public string StartedModulusStr { get; set; }
        public string StatedExponentStr { get; set; }
        public int Accepted { get; set; }
        public int? AcceptedDeviceId { get; set; }
        public string? AcceptedModulusStr { get; set; }
        public string? AcceptedExponentStr { get; set; }
        public DateTime LastMessage { get; set; }
    }
}
