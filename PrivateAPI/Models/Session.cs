using System;
namespace PrivateAPI.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string SymmetricKey { get; set; }
        public string InitVector { get; set; }
        public DateTime LastUsed { get; set; }
    }
}
