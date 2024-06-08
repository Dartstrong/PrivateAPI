using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateAPI.Models
{
    public class LastEntryDate
    {
        public int Id { get; set; }
        public int LoginHistoryId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
