namespace PrivateAPI.Models
{
    public class AcceptingStartDialogue
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public int StartedDeviceId { get; set; }
        public string Receiver { get; set; }
        public int AcceptedDeviceId { get; set; }
        public string PublicKeyModulusStr { get; set; }
        public string PublicKeyExponentStr { get; set; }
    }
}
