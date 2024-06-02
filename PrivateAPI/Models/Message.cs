using System;

namespace PrivateAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int DialogueId { get; set; }
        public int Sender { get; set; }
        public string SenderData { get; set; }
        public int Receiver { get; set; } 
        public string ReceiverData { get; set; }
        public DateTime ReceivedServer { get; set; }

    }
}
