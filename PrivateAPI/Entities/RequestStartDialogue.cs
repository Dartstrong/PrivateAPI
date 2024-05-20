﻿namespace PrivateAPI.Entities
{
    public class RequestStartDialogue
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string SenderdDeviceId { get; set; }
        public string SenderPassword { get; set; }
        public string Receiver { get; set; }
        public string PublicKeyModulus{ get; set; }
        public string PublicKeyExponent { get; set; }
    }
}
