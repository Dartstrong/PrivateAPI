using System;

namespace PrivateAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public int SenderDeviceId { get; set; }
        public string Receiver { get; set; }
        public int ReceiverDeviceId { get; set; }
        public string Data { get; set; }
        public DateTime? ReceivedServer { get; set; }
    }
}
