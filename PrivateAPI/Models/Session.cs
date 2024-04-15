using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string SymmetricKey { get; set; }
        public string InitVector { get; set; }
    }
}
