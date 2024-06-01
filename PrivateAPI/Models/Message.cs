using System;

namespace PrivateAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int DialogueId { get; set; }
        public string Data { get; set; }
        public DateTime ReceivedServer { get; set; }
        public int Sender { get; set; }
    }
}
